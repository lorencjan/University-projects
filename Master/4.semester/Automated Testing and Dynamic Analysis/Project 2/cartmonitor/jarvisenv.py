"""
Jarvis factory simulation environment.
"""

import factory
import sched

class Jarvis(factory.Factory):
    "Jarvis factory"

    TRACKS = factory.Tracks([
        # From -> To -> Cost
        factory.Track('A', 'B', 20),
        factory.Track('B', 'A', 30),
        factory.Track('B', 'C', 20),
        factory.Track('C', 'D', 20),
        factory.Track('D', 'A', 10),
    ])

    SCHED = None
    SIMULATION_TIME = 0

    @staticmethod
    def get_tracks():
        "returns all the Jarvis tracks"
        return Jarvis.TRACKS

    @staticmethod
    def reset_scheduler():
        "resets scheduler for text fixture"
        Jarvis.SIMULATION_TIME = 0
        Jarvis.SCHED = sched.scheduler(Jarvis.time, Jarvis._sleep)

    @staticmethod
    def plan(when: int, event, argument=(), kwargs={}):
        "plans the event of the factory"
        Jarvis.SCHED.enter(when, 0, event, argument, kwargs)
    
    @staticmethod
    def run():
        "runs the plan"
        Jarvis.SCHED.run()

    @staticmethod
    def time():
        "returns absolute simulation time"
        return Jarvis.SIMULATION_TIME

    @staticmethod
    def _sleep(delay):
        "force simulation time to progress"
        Jarvis.SIMULATION_TIME += delay