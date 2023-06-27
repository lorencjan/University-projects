#!/usr/bin/env python3
"""
Example of usage/test of Cart controller implementation.
"""

import sys
from cartctl_new import CartCtl, PriorityTimeExceededException, CannotProcessRequestException, Status
from cart import Cart, CargoReq
from jarvisenv import Jarvis
import unittest

def log(msg):
    "simple logging"
    print('  %s' % msg)

class TestCartRequests(unittest.TestCase):

    @staticmethod
    def set_request(ctl: CartCtl, cargo: CargoReq, delay = None) -> None:
        """ Simple callback to create a transport request """

        ctl.request(cargo)
        if delay:  # helps to set the priority - simulates passing of time
            Jarvis._sleep(delay)

    def assert_empty(self, ctl: CartCtl) -> None:
        """ Asserts idle empty state of a cart """

        self.assertListEqual(ctl.requests, [])
        self.assertEqual(ctl.status, Status.Idle)
        self.assertTrue(ctl.cart.empty())
        self.assertEqual(ctl.cart.load_sum(), 0)

    def test_happy(self):
        "Happy-path test"

        def add_load(c: CartCtl, cargo_req: CargoReq):
            "callback for schedulled load"
            log('%d: Requesting %s at %s' % \
                (Jarvis.time(), cargo_req, cargo_req.src))
            c.request(cargo_req)

        def on_move(c: Cart):
            "example callback (for assert)"
            # put some asserts here
            log('%d: Cart is moving %s->%s' % (Jarvis.time(), c.pos, c.data))

        def on_load(c: Cart, cargo_req: CargoReq):
            "example callback for logging"
            log('%d: Cart at %s: loading: %s' % (Jarvis.time(), c.pos, cargo_req))
            log(c)
            cargo_req.context = "loaded"

        def on_unload(c: Cart, cargo_req: CargoReq):
            "example callback (for assert)"
            # put some asserts here
            log('%d: Cart at %s: unloading: %s' % (Jarvis.time(), c.pos, cargo_req))
            log(c)
            self.assertEqual('loaded', cargo_req.context)
            cargo_req.context = 'unloaded'
            if cargo_req.content == 'helmet':
                self.assertEqual('B', c.pos)
            if cargo_req.content == 'heart':
                self.assertEqual('A', c.pos)
            #if cargo_req.content.startswith('bracelet'):
            #    self.assertEqual('C', c.pos)
            if cargo_req.content == 'braceletR':
                self.assertEqual('A', c.pos)
            if cargo_req.content == 'braceletL':
                self.assertEqual('C', c.pos)

        # Setup Cart
        # 4 slots, 150 kg max payload capacity, 2=max debug
        cart_dev = Cart(4, 150, 0)
        cart_dev.onmove = on_move

        # Setup Cart Controller
        c = CartCtl(cart_dev, Jarvis)

        # Setup Cargo to move
        helmet = CargoReq('A', 'B', 20, 'helmet')
        helmet.onload = on_load
        helmet.onunload = on_unload

        heart = CargoReq('C', 'A', 40, 'heart')
        heart.onload = on_load
        heart.onunload = on_unload

        braceletR = CargoReq('D', 'A', 40, 'braceletR')
        braceletR.onload = on_load
        braceletR.onunload = on_unload

        braceletL = CargoReq('D', 'C', 40, 'braceletL')
        braceletL.onload = on_load
        braceletL.onunload = on_unload

        # Setup Plan
        Jarvis.reset_scheduler()
        #         when  event     called_with_params
        Jarvis.plan(10, add_load, (c,helmet))
        Jarvis.plan(45, add_load, (c,heart))
        Jarvis.plan(40, add_load, (c,braceletR))
        Jarvis.plan(25, add_load, (c,braceletL))
        
        # Exercise + Verify indirect output
        #   SUT is the Cart.
        #   Exercise means calling Cart.request in different time periods.
        #   Requests are called by add_load (via plan and its scheduler).
        #   Here, we run the plan.
        Jarvis.run()

        # Verify direct output
        log(cart_dev)
        self.assertTrue(cart_dev.empty())
        self.assertEqual('unloaded', helmet.context)
        self.assertEqual('unloaded', heart.context)
        self.assertEqual('unloaded', braceletR.context)
        self.assertEqual('unloaded', braceletL.context)
        #self.assertEqual(cart_dev.pos, 'C')

    """ ########## CEG TESTS ########## """

    def test_exception_is_thrown_if_cart_cannot_handle_request_due_to_unavailable_slots(self):
        """ Covers CEG 1,2,3,8
            Cart cannot process request if:
              a) it has no available slots - this test
              b) request weight would exceed cart's capacity
              c) cart is in UnloadOnly state
            CEG cases 1-8 cover all combinations but as any of these are enough to fail and
            assignment tells 5-15 tests which I already exceed, individual tests suffice
        """

        # arrange
        ctl = CartCtl(Cart(2, 50), Jarvis)
        request = lambda name: CargoReq("A", "B", 50, name)
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.set_request, (ctl, request("Slot 1 item")))        
        Jarvis.plan(5, self.set_request, (ctl, request("Slot 2 item")))        
        Jarvis.plan(10, self.set_request, (ctl, request("Excessive item")))

        # assert
        self.assertRaises(CannotProcessRequestException, Jarvis.run)
    
    def test_exception_is_thrown_if_cart_cannot_handle_request_due_to_excessive_weight(self):
        """ Covers CEG 1,6,7,8
            Cart cannot process request if:
              a) it has no available slots
              b) request weight would exceed cart's capacity - this test
              c) cart is in UnloadOnly state
            CEG cases 1-8 cover all combinations but as any of these are enough to fail and
            assignment tells 5-15 tests which I already exceed, individual tests suffice
        """

        # arrange
        ctl = CartCtl(Cart(2, 50), Jarvis)
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.set_request, (ctl, CargoReq("A", "B", 51, "excessive weight")))

        # assert
        self.assertRaises(CannotProcessRequestException, Jarvis.run)
    
    def test_exception_is_thrown_if_cart_cannot_handle_request_being_in_unload_only_state(self):
        """ Covers CEG 1,2,5,6
            Cart cannot process request if:
              a) it has no available slots
              b) request weight would exceed cart's capacity
              c) cart is in UnloadOnly state - this test
            CEG cases 1-8 cover all combinations but as any of these are enough to fail and
            assignment tells 5-15 tests which I already exceed, individual tests suffice
        """

        # arrange
        ctl = CartCtl(Cart(2, 50), Jarvis)
        request = lambda name: CargoReq("A", "B", 50, name)
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.set_request, (ctl, request("whatever"), 60))
        Jarvis.plan(65, self.set_request, (ctl, request("cannot load")))

        # assert
        self.assertRaises(CannotProcessRequestException, Jarvis.run)

    def test_cart_should_do_nothing_if_no_transport_is_requested(self) -> None:
        """ Covers CEG.9
            No request comes -> nothing happens.
        """

        # arrange
        cart = Cart(2, 50)
        cart.onmove = lambda x: self.fail("There should be no movement")
        ctl = CartCtl(cart, Jarvis)
        Jarvis.reset_scheduler()

        # act
        Jarvis.run()

        # assert
        self.assert_empty(ctl)
        self.assertEqual(ctl.cart.pos, "A")
    
    def test_successful_load_of_normal_material(self):
        """ Covers CEG 10, 11, 14 (same path, always just one step further)
            Checks if a request get accepted and loaded in time.       
            (Individual tests could be made but as 5-15 tests suffice I grouped these as they have same path) 
        """

        def assert_on_load(cart: Cart, cargo: CargoReq) -> None:
            """ Checks that cargo is loaded in time - CEG.14 """

            self.assertIn(cargo, cart.slots)
            self.assertEqual("whatever", cargo.content)
            self.assertLess(Jarvis.time(), 61)

        # arrange
        ctl = CartCtl(Cart(2, 50), Jarvis)
        cargo = CargoReq("A", "B", 25, "whatever")
        cargo.onload = assert_on_load
        Jarvis.reset_scheduler()

        # act + assert
        Jarvis.plan(0, self.assert_empty, [ctl])
        Jarvis.plan(1, self.set_request, (ctl, cargo))
        Jarvis.plan(2, self.assertIn, (cargo, ctl.requests))  # checks that request got accepted - CEG.11
        Jarvis.run()

        # assert
        self.assert_empty(ctl)               # request is finished
        self.assertEqual(ctl.cart.pos, "B")  # arrived to destination
        
    def test_successful_load_of_priority_material(self):
        """ Covers CEG 12, 15 (same path, 15 is just one step further)
            Checks if a request get accepted and loaded not in time but withing another minute (priority time).       
            (Individual tests could be made but as 5-15 tests suffice I grouped these as they have same path) 
        """

        def assert_on_load(cart: Cart, cargo: CargoReq) -> None:
            """ Checks that cargo is marked with priority property and is loaded in time - CEG.15 """

            self.assertIn(cargo, cart.slots)
            self.assertEqual("whatever", cargo.content)
            self.assertTrue(cargo.prio)
            self.assertLess(Jarvis.time(), 121)

        def assert_priority(ctl: CartCtl, cargo: CargoReq) -> None:
            """ Assers if priority property is set """

            self.assertTrue(cargo.prio)
            self.assertEqual(ctl.status, Status.UnloadOnly)

        # arrange
        ctl = CartCtl(Cart(2, 50), Jarvis)
        cargo = CargoReq("A", "B", 25, "whatever")
        cargo.onload = assert_on_load
        Jarvis.reset_scheduler()

        # act + assert
        Jarvis.plan(0, self.assert_empty, [ctl])
        Jarvis.plan(1, self.set_request, (ctl, cargo, 60))
        Jarvis.plan(65, assert_priority, (ctl, cargo))  # changed to priority - CEG.12
        Jarvis.run()

        # assert
        self.assert_empty(ctl)
        self.assertEqual(ctl.cart.pos, "B")

    def test_exception_is_thrown_if_priority_time_is_exceeded(self):
        """ Covers CEG.13
            Checks if request fails after exceeding 1 min for priority material.        
        """

        # arrange
        ctl = CartCtl(Cart(2, 50), Jarvis)
        cargo = CargoReq("A", "B", 25, "whatever")
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.set_request, (ctl, cargo, 120))

        # assert
        self.assertRaises(PriorityTimeExceededException, Jarvis.run)


    """ ########## COMBINATION TESTS ########## """

    def test_combination_1(self):
        """ Covers combination 1
            1 slot | 150 cap | cart at max weight -> should not accept | request overweight -> should not accept | now | direct path
        """

        # arrange
        cart = Cart(1, 150)  # 1 slot, 150 cap
        cart.slots[0] = CargoReq("A", "D", 150, "prefilled material")  # cart at max weight
        ctl = CartCtl(cart, Jarvis)
        cargo = CargoReq("A", "B", 151, "exceeding weight + direct path")
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.set_request, (ctl, cargo))  # now

        # assert
        self.assertRaises(CannotProcessRequestException, Jarvis.run)
    
    def test_combination_2(self):
        """ Covers combination 2
            1 slot | 500 cap | cart not at max weight | request not overweight | future | longer path
        """

        request_name = "not exceeding weight + longer path"

        def assert_on_load(cart: Cart, cargo: CargoReq) -> None:
            """ Cargo is loaded in time """

            self.assertIn(cargo, cart.slots)
            self.assertEqual(request_name, cargo.content)
            self.assertLess(Jarvis.time(), 61)

        # arrange
        ctl = CartCtl(Cart(1, 500), Jarvis)  # 1 slot, 500 cap, empty (not at max weight)
        cargo = CargoReq("A", "C", 450, request_name)
        cargo.onload = assert_on_load
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.assert_empty, [ctl])
        Jarvis.plan(10, self.set_request, (ctl, cargo))  # future
        Jarvis.plan(11, self.assertIn, (cargo, ctl.requests))
        Jarvis.run()

        # assert
        self.assert_empty(ctl)

    def test_combination_3(self):
        """ Covers combination 3
            2 slots | 50 cap | cart at max weight -> should not accept | request overweight -> should not accept | future | longer path
        """

        # arrange
        cart = Cart(2, 50)  # 2 slots, 50 cap
        cart.slots[0] = CargoReq("A", "D", 50, "prefilled material")  # cart at max weight
        ctl = CartCtl(cart, Jarvis)
        cargo = CargoReq("A", "C", 51, "exceeding weight + longer path")
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(10, self.set_request, (ctl, cargo))  # future

        # assert
        self.assertRaises(CannotProcessRequestException, Jarvis.run)
    
    def test_combination_4(self):
        """ Covers combination 4
            2 slot | 150 cap | cart not at max weight | request not overweight | now | direct path
        """

        request_name = "not exceeding weight + direct path"

        def assert_on_load(cart: Cart, cargo: CargoReq) -> None:
            """ Cargo is loaded in time """

            self.assertIn(cargo, cart.slots)
            self.assertEqual(request_name, cargo.content)
            self.assertLess(Jarvis.time(), 61)

        # arrange
        ctl = CartCtl(Cart(2, 150), Jarvis)  # 2 slots, 150 cap, empty (not at max weight)
        cargo = CargoReq("A", "B", 100, request_name)
        cargo.onload = assert_on_load
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.assert_empty, [ctl])
        Jarvis.plan(0, self.set_request, (ctl, cargo))  # now
        Jarvis.plan(1, self.assertIn, (cargo, ctl.requests))
        Jarvis.run()

        # assert
        self.assert_empty(ctl)

    def test_combination_5(self):
        """ Covers combination 5
            2 slots | 500 cap | cart at max weight -> should not accept | request overweight -> should not accept | now | direct path
        """

        # arrange
        cart = Cart(2, 500)  # 2 slots, 500 cap
        cart.slots[0] = CargoReq("A", "D", 500, "prefilled material")  # cart at max weight
        ctl = CartCtl(cart, Jarvis)
        cargo = CargoReq("A", "B", 501, "exceeding weight + direct path")
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.set_request, (ctl, cargo))  # now

        # assert
        self.assertRaises(CannotProcessRequestException, Jarvis.run)

    def test_combination_6(self):
        """ Covers combination 6
            3 slots | 50 cap | cart not at max weight | request overweight -> should not accept | now | direct path
        """

        # arrange
        ctl = CartCtl(Cart(3, 50), Jarvis)  # 3 slots, 50 cap, empty (not at max weight)
        cargo = CargoReq("A", "B", 51, "exceeding weight + direct path")
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.set_request, (ctl, cargo))  # now

        # assert
        self.assertRaises(CannotProcessRequestException, Jarvis.run)

    def test_combination_7(self):
        """ Covers combination 7
            3 slots | 150 cap | cart at max weight -> should not accept | request overweight -> should not accept | future | longer path
        """

        # arrange
        cart = Cart(3, 150)
        cart.slots[0] = CargoReq("A", "B", 75, "prefilled material 1")  # cart at max weight
        cart.slots[1] = CargoReq("A", "B", 75, "prefilled material 2")  # this time with more than 1 slot
        ctl = CartCtl(cart, Jarvis) 
        cargo = CargoReq("A", "D", 151, "exceeding weight + longer path")
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(10, self.set_request, (ctl, cargo))  # future

        # assert
        self.assertRaises(CannotProcessRequestException, Jarvis.run)

    def test_combination_8(self):
        """ Covers combination 8
            4 slots | 50 cap | cart at max weight -> should not accept | request overweight -> should not accept | now | longer path
        """

        # arrange
        cart = Cart(4, 50)
        cart.slots[0] = CargoReq("A", "B", 20, "prefilled material 1")  # cart at max weight
        cart.slots[1] = CargoReq("A", "B", 20, "prefilled material 2")  # this time with
        cart.slots[1] = CargoReq("A", "B", 10, "prefilled material 3")  # 3 slots
        ctl = CartCtl(cart, Jarvis) 
        cargo = CargoReq("A", "D", 51, "exceeding weight + longer path")
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.set_request, (ctl, cargo))  # now

        # assert
        self.assertRaises(CannotProcessRequestException, Jarvis.run)

    def test_combination_9(self):
        """ Covers combination 9
            4 slots | 150 cap | cart not at max weight | request not overweight | future | direct path
        """

        request_name = "not exceeding weight + direct path"

        def assert_on_load(cart: Cart, cargo: CargoReq) -> None:
            """ Cargo is loaded in time """

            self.assertIn(cargo, cart.slots)
            self.assertEqual(request_name, cargo.content)
            self.assertLess(Jarvis.time(), 61)

        # arrange
        ctl = CartCtl(Cart(4, 150), Jarvis)  # 4 slots, 150 cap, empty (not at max weight)
        cargo = CargoReq("A", "B", 100, request_name)
        cargo.onload = assert_on_load
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.assert_empty, [ctl])
        Jarvis.plan(10, self.set_request, (ctl, cargo))  # future
        Jarvis.plan(11, self.assertIn, (cargo, ctl.requests))
        Jarvis.run()

        # assert
        self.assert_empty(ctl)

    def test_combination_10(self):
        """ Covers combination 10
            3 slots | 50 cap | cart not at max weight | request not overweight | now | direct path
        """

        request_name = "not exceeding weight + direct path"

        def assert_on_load(cart: Cart, cargo: CargoReq) -> None:
            """ Cargo is loaded in time """

            self.assertIn(cargo, cart.slots)
            self.assertEqual(request_name, cargo.content)
            self.assertLess(Jarvis.time(), 61)

        # arrange
        ctl = CartCtl(Cart(3, 50), Jarvis)  # 2 slots, 50 cap, empty (not at max weight)
        cargo = CargoReq("A", "B", 50, request_name)
        cargo.onload = assert_on_load
        Jarvis.reset_scheduler()

        # act
        Jarvis.plan(0, self.assert_empty, [ctl])
        Jarvis.plan(0, self.set_request, (ctl, cargo))  # now
        Jarvis.plan(1, self.assertIn, (cargo, ctl.requests))
        Jarvis.run()

        # assert
        self.assert_empty(ctl)

if __name__ == "__main__":
    unittest.main()
