"""
Implementation of a Cart in a robotic factory.
"""

import enum
import jarvisenv

class Status(enum.Enum):
    "Available cart statuses"
    Idle = 0
    Moving = 1
    Loading = 2
    Unloading = 3

class CargoReq:
    "Object for a single cargo"
    def __init__(self, src, dst, weight, content):
        assert weight > 0
        self.src = src
        self.dst = dst
        self.weight = weight
        self.content = content
        self.prio = False
        self.born = 0
        # event call-backs (post events)
        self.onload = CargoReq.just_pass_it
        self.onunload = CargoReq.just_pass_it
        # user defined context
        self.context = None

    def __str__(self):
        return '%sCargoReq(%s)' % ("Priority" if self.prio else "", self.content)

    def set_priority(self):
        "one way setting of the priority"
        self.prio = True

    def load(self, cart_dev):
        "load itself, invoke callback"
        if callable(self.onload):
            self.onload(cart_dev, self)

    def unload(self, cart_dev):
        "unload itself, invoke callback"
        if callable(self.onunload):
            self.onunload(cart_dev, self)

    def just_pass_it(self, argument=None):
        "Dummy function (callback) for load and unload events"

class CartError(Exception):
    "Exception for some self-checks in Cart class"

class Cart:
    "Cart device"
    def __init__(self, nslots, load_capacity, debug_lvl=0):
        self.slots = [None] * nslots
        self.load_capacity = load_capacity
        self.status = Status.Idle
        self.data = None
        self.pos = None
        self.debug_lvl = debug_lvl
        self.onmove = Cart.just_pass_it

    def __str__(self):
        return 'Cart(pos=%s, %s, data=%s, maxload=%d, slots=%s)' % \
                (self.pos, self.status, self.data, self.load_capacity,
                 [str(c) for c in self.slots])

    def just_pass_it(self, argument=None):
        "Dummy function for a move"

    def log(self, msg):
        "a simple logger"
        if self.debug_lvl > 1:
            print(self)
        if self.debug_lvl > 0:
            print(msg)

    def check_idle(self):
        if self.status != Status.Idle:
            raise CartError("Cart is busy: %s" % self.status)

    def empty(self):
        "returns True if cart has no cargo at all"
        return self.slots == [None] * len(self.slots)

    def load_sum(self):
        "return sum load of all cargos"
        sum_weight = 0
        for slot in self.slots:
            if slot:
                sum_weight += slot.weight
        return sum_weight

    def any_prio_cargo(self):
        "returns True if cart has any pritoritized cargo"
        for slot in self.slots:
            if slot and slot.prio:
                return True
        return False

    def get_prio_idx(self):
        "returns index of slot index with prioritized cargo or -1 if there is none"
        for i in range(len(self.slots)):
            if self.slots[i].prio:
                return i
        return -1

    def check_free_slot(self, slot):
        "pass or raise an exception about invalid slot number"
        if slot < 0 or slot >= len(self.slots):
            raise IndexError("slot '%s' outside range [0;%d]" % \
                    (slot, len(self.slots)))
        if self.slots[slot] is not None:
            raise ValueError("slot %d not empty: %s" % \
                    (slot, self.slots[slot]))

    def check_loaded_slot(self, slot):
        "pass or raise an exception about invalid slot when unloading"
        if slot < 0 or slot >= len(self.slots):
            raise IndexError("slot '%s' outside range [0;%d]" % \
                    (slot, len(self.slots)))
        if self.slots[slot] is None:
            raise ValueError("slot %d not empty: %s" % \
                    (slot, self.slots[slot]))

    def get_free_slot(self):
        "returns index of free slot, or -1 if all slots are occupied"
        for i in range(len(self.slots)):
            if self.slots[i] is None:
                return i
        return -1

    def set_idle(self):
        "helper function to idle the cart"
        self.log("setting Idle")
        self.status = Status.Idle
        self.data = None

    def start_moving(self, destination):
        self.log("at %s, starting moving to %s" % (self.pos, destination))
        self.check_idle()
        self.status = Status.Moving
        self.data = destination
        if callable(self.onmove):
            self.onmove(self)
        #self.log("started")

    def finish_moving(self):
        self.log("finishing moving to %s" % self.data)
        assert self.status == Status.Moving
        self.pos = self.data
        self.set_idle()
        #self.log("finished")

    def start_loading(self, cargo_req, slot):
        self.log("start loading: %s" % cargo_req)
        self.check_idle()
        self.check_free_slot(slot)
        self.status = Status.Loading
        self.data = (cargo_req, slot)
        #self.log("started")
        # here, a factory can start loading to the slot

    def finish_loading(self):
        self.log("finishing loading")
        assert self.status == Status.Loading
        cargo_req, slot = self.data
        self.slots[slot] = cargo_req
        self.set_idle()
        #self.log("finished")
        cargo_req.load(self)
        return cargo_req

    def start_unloading(self, slot):
        self.log("start unloading")
        self.check_idle()
        self.check_loaded_slot(slot)
        self.status = Status.Unloading
        self.data = slot
        self.log("started")
        # here, a factory can start unloading the slot

    def finish_unloading(self):
        self.log("finishing unloading")
        assert self.status == Status.Unloading
        cargo_req = self.slots[self.data]
        self.slots[self.data] = None
        self.set_idle()
        self.log("finished")
        cargo_req.unload(self)
        return cargo_req
