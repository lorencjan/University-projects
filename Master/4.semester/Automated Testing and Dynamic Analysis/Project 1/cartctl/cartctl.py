#!/usr/bin/env python3
"""
Cart Controller in a factory
"""
import enum
import sched
import collections
import cart

class Status(enum.Enum):
    "Controller status"
    Normal = 0      # normal behaviour
    UnloadOnly = 1  # must unload only
    Idle = 2        # controller does not know what to do at the moment

class CartCtl:
    "Cart controller"
    def __init__(self, cart_device, factory):
        self.cart = cart_device
        self.time = factory.time
        self.plan = factory.plan
        self.tracks = factory.get_tracks()
        if self.cart.pos is None and self.tracks:
            self.cart.pos = list(self.tracks.stations())[0]
        self.requests = []
        self.inprogress = []
        self.status = Status.Idle
        self.only_unload = False

    def request(self, new_cargo: cart.CargoReq):
        "enqueue a new request for transfer"
        self.requests.append(new_cargo)
        new_cargo.born = self.time()
        if self.status == Status.Idle:
            self.plan(0, self.heartbeat)

    def sched_unload(self, slot):
        "schedule the unload"
        # should not be constrained
        # from now, after 2 tics, perform unload
        self.cart.start_unloading(slot)
        self.plan(2, self.perform_unload)

    def perform_unload(self):
        "proceed unloading current slot"
        self.cart.finish_unloading()
        self.plan(0, self.heartbeat)

    def try_unload_here_single(self):
        "unload what is possible"
        for slot in range(len(self.cart.slots)):
            cargo_req = self.cart.slots[slot]
            if cargo_req is None:
                continue
            if cargo_req.dst == self.cart.pos:
                self.sched_unload(slot)
                return slot
        return -1

    def sched_load(self, slot, request_idx):
        "schedule the load"
        self.cart.start_loading(self.requests[request_idx], slot)
        self.plan(2, self.perform_load)

    def perform_load(self):
        "proceed loading some cargo from current pos"
        cargo_req = self.cart.finish_loading()
        self.requests.remove(cargo_req)
        self.plan(0, self.heartbeat)

    def try_load_here_single(self):
        "load here to a single slot, if there is something here"
        free_slot = self.cart.get_free_slot()
        if free_slot == -1:
            # no free slot
            return -1
        free_cap = self.cart.load_capacity - self.cart.load_sum()
        if free_cap <= 0:
            # no free capacity
            return -1
        for request_idx in range(len(self.requests)):
            request = self.requests[request_idx]
            if request.src == self.cart.pos and request.weight <= free_cap:
                if self.status != Status.UnloadOnly:
                    self.sched_load(free_slot, request_idx)
                    return free_slot
        return -1

    def sched_move(self, track):
        "schedule a move"
        self.cart.start_moving(track.dst)
        self.plan(track.cost, self.perform_move)

    def perform_move(self):
        "move has been done"
        self.cart.finish_moving()
        self.plan(0, self.heartbeat)

    def find_load_there_single(self, priority=False):
        """
        Finds a single slot and requests that fits to together.
        Returns a pair of free_slot_idx and request_idx, or (-1,-1) otherwise.
        """
        free_slot = self.cart.get_free_slot()
        if free_slot == -1:
            # no free slot
            return (-1, -1)
        free_cap = self.cart.load_capacity - self.cart.load_sum()
        if free_cap <= 0:
            # no free capacity
            return (-1, -1)
        for request_idx in range(len(self.requests)):
            request = self.requests[request_idx]
            if request.weight <= free_cap:
                if self.status != Status.UnloadOnly:
                    return (free_slot, request_idx)
        return (-1, -1)

    def sort_requests(self):
        "sorts requests wrt. priority"
        self.requests.sort(key=lambda i: (1-i.prio)*10**6+i.born, reverse=True)

    def update_prio_requests(self):
        "update prio in each of requests if it waits too long"
        curr_time = self.time()
        for cargo_req in self.requests:
            if curr_time - cargo_req.born >= 60:
                cargo_req.set_priority()
        self.sort_requests()

    def find_prio_request(self):
        "return prioritized Load waiting in requests, or None."
        for cargo_req in self.requests:
            if cargo_req.prio:
                return cargo_req
        return None

    def check_dead_requests(self):
        "remove normal request if it is too big or prioritized request if it waits too long."
        curr_time = self.time()
        for cargo_req in self.requests:
            wait_time = curr_time - cargo_req.born
            if cargo_req.prio == True and wait_time >= 120:
                self.requests.remove(cargo_req)
            if cargo_req.weight > self.cart.load_capacity:
                self.requests.remove(cargo_req)

    def heartbeat(self):
        "main controlling loop"
        # update environment
        self.update_prio_requests()
        # precistenie poziadaviek
        self.check_dead_requests()

        if self.cart.any_prio_cargo():
            self.status = Status.UnloadOnly
        else:
            self.status = Status.Normal

        slot = self.try_unload_here_single()
        if slot != -1:
            # should be scheduled for unloading
            return
        if self.status != Status.UnloadOnly:
            slot = self.try_load_here_single()
            if slot != -1:
                # should be scheduled for loading
                return

        # nothing for load or unload at this station

        #    if just a single slot is available, and in prio -> 1 step of move
        (free_slot, request_idx) = self.find_load_there_single(True)
        if free_slot >= 0:
            # found somewhere, let's go there!
            request = self.requests[request_idx]
            path = self.tracks.get_path(self.cart.pos, request.src)
            if path:
                self.sched_move(path[0])
                return

        # try move with cargo to destination
        if not self.cart.empty():
            paths = self.evaluate_all_paths()
            fast_candidate_idx = CartCtl.find_fastest_slot(paths)
            assert fast_candidate_idx is not None
            first_track = paths[fast_candidate_idx][0]
            self.sched_move(first_track)
            return

        # try move for a load to source
        (free_slot, request_idx) = self.find_load_there_single(False)
        if free_slot >= 0:
            # found somewhere, let's go there!
            request = self.requests[request_idx]
            path = self.tracks.get_path(self.cart.pos, request.src)
            if path:
                self.sched_move(path[0])
                return

        print('Do not know what to do at time %s' % self.time())
        self.status = Status.Idle

    def evaluate_all_paths(self):
        "finds all paths for all cargo"
        paths = [self.tracks.get_path(self.cart.pos, l.dst) if l else None \
            for l in self.cart.slots]
        return paths

    @staticmethod
    def eval_cost(path):
        "calculate the cost of the path"
        sum_cost = 0
        for track in path:
            sum_cost += track.cost
        return sum_cost

    @staticmethod
    def find_fastest_slot(paths):
        "finds index of the occupied slot with shortest destination"
        costs = [CartCtl.eval_cost(path) if path else None for path in paths]
        minidx = None
        for idx in range(len(costs)):
            if costs[idx] is not None:
                if minidx is None:
                    minidx = idx
                elif costs[idx] < costs[minidx]:
                    minidx = idx
        return minidx
