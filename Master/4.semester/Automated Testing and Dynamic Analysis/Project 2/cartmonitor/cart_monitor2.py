#!/usr/bin/env python3

# Project: VUT FIT ATA Project - Řízení vozíku v robotické továrně
# Author: Dominik Harmim <xharmi00@stud.fit.vutbr.cz>
# Year: 2021
# Description: Dynamic analyser of a cart controller.

CAPACITY = 150
NSLOTS = 4
COVERAGE_MATRIX = {
    'A': NSLOTS * [False],
    'B': NSLOTS * [False],
    'C': NSLOTS * [False],
    'D': NSLOTS * [False],
}


def report_coverage() -> None:
    """ Coverage reporter """
    global NSLOTS, COVERAGE_MATRIX
    covered = 0
    for _, slots in COVERAGE_MATRIX.items():
        for slot in slots:
            covered += 1 if slot else 0
    coverage = covered / (NSLOTS * len(COVERAGE_MATRIX)) * 100
    print(f'CartCoverage {coverage}%')


LOADED_SLOTS = NSLOTS * [False]
CURRENT_CAPACITY = 0


def onmoving(time, pos1, pos2) -> None:
    ...


def onloading(time, pos, content, w, slot) -> None:
    global NSLOTS, COVERAGE_MATRIX, LOADED_SLOTS, CURRENT_CAPACITY, CAPACITY
    slot = int(slot)
    time = int(time)
    w = int(w)

    # coverage
    if pos in COVERAGE_MATRIX and 0 <= slot < NSLOTS:
        COVERAGE_MATRIX[pos][slot] = True

    if 0 <= slot < NSLOTS:
        # property 1
        if LOADED_SLOTS[slot]:
            print(f'{time}:error: loading into an occupied slot #{slot}')
        LOADED_SLOTS[slot] = True

    # property 7
    CURRENT_CAPACITY += w
    if CURRENT_CAPACITY > CAPACITY:
        print(f'{time}:error: the cart is overloaded')


def onunloading(time, pos, content, w, slot) -> None:
    global LOADED_SLOTS, CURRENT_CAPACITY
    slot = int(slot)
    w = int(w)

    if 0 <= slot < NSLOTS:
        # property 2
        if not LOADED_SLOTS[slot]:
            print(f'{time}:error: unloading from an empty slot #{slot}')
        LOADED_SLOTS[slot] = False

    CURRENT_CAPACITY -= w


def onevent(event) -> None:
    """ Event handler. event = [TIME, EVENT_ID, ...] """
    event_id = event[1]
    del event[1]

    if event_id == 'moving':
        onmoving(*event)
    elif event_id == 'loading':
        onloading(*event)
    elif event_id == 'unloading':
        onunloading(*event)


def monitor(reader):
    """ Main function """
    for line in reader:
        line = line.strip()
        onevent(line.split())
    report_coverage()


if __name__ == "__main__":
    import sys
    monitor(sys.stdin)