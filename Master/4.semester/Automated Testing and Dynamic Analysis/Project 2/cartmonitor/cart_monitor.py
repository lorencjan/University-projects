#!/usr/bin/env python3

"""
Dynamic analyser of a cart controller.
"""

SLOTS = 4
SLOTS_STATE = [None] * SLOTS  # stores destinations of requests to monitor property 3
MAX_CAPACITY = 150
CAPACITY = 0
REQUESTS = []  # stores requests to keep track for load/unload
COVERAGE = {station: [False] * SLOTS for station in ["A", "B", "C", "D"]}

class Request():
    def __init__(self, time, src, dst, content, w):
        self.time = time
        self.src = src
        self.dst = dst
        self.content = content
        self.w = w
    
    def compare(self, src, content, w):
        return self.src == src and self.content == content and self.w == w

def report_coverage():
    "Coverage reporter"
    # !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    # Zde nahradte vypocet/vypis aktualne dosazeneho pokryti
    global COVERAGE, SLOTS
    covered = sum([int(slot) for slots in COVERAGE.values() for slot in slots])
    total = len(COVERAGE) * SLOTS
    print('CartCoverage %d%%' % ((covered/total)*100))

def onmoving(time, pos1, pos2):
    "priklad event-handleru pro udalost moving"
    # Podobnou funkci muzete i nemusite vyuzit, viz onevent().
    # Vsechny parametry jsou typu str; nektere muze byt nutne pretypovat.
    global SLOTS_STATE

    # property 3 - cart must be unloaded in the load destination
    # (attempt to move while this position is a destination for a load in a slot)
    if (pos1 in SLOTS_STATE):
        print(f'{time}:error: the cart cannot leave a station before it unloads there')

def onrequesting(time, src, dst, content, w):
    global REQUESTS
    REQUESTS.append(Request(time, src, dst, content, w))

def onloading(time, pos, content, w, slot):
    global REQUESTS, CAPACITY, MAX_CAPACITY, SLOTS_STATE, COVERAGE
    request = next(iter(r for r in REQUESTS if r.compare(pos, content, w)), None)
    if request is None:  # property 5 - cannot load if no request was made
        print(f'{time}:error: the cart tries to load despite of not having a request for it')
    else:
        REQUESTS.remove(request)  # request is loaded ~ handled (we don't need it on unload)

    CAPACITY += int(w)
    if CAPACITY > MAX_CAPACITY:  # property 7 - check overload
        print(f'{time}:error: the cart exceeded its capacity')

    slot = int(slot)
    if 0 <= slot <= SLOTS:
        if SLOTS_STATE[slot] is not None:  # property 1 - cannot load into a loaded slot
            print(f'{time}:error: loading into an occupied slot #{slot}')
        
        SLOTS_STATE[slot] = request.dst if request else "unknown"
        if pos in COVERAGE.keys():
            COVERAGE[pos][slot] = True
    else:
        # property 6 - cannot load more than 4 times ~ number of slots
        # partially handled also by prop 1, if all 4 slots are loaded and additional fifth
        # load is done, it is error of a loaded slot but also covers this prop's >4 load
        print(f'{time}:error: the cart tries to load outside of available slots')
    
def onunloading(time, pos, content, w, slot):
    global CAPACITY, SLOTS_STATE
    CAPACITY -= int(w)
    slot = int(slot)
    if not (0 <= slot < SLOTS):
        return

    if SLOTS_STATE[slot] is None:  # property 2 - cannot unload from an empty slot
        print(f'{time}:error: unloading from an empty slot #{slot}')
    
    SLOTS_STATE[slot] = None

def onevent(event):
    "Event handler. event = [TIME, EVENT_ID, ...]"
    # !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    # ZDE IMPLEMENTUJTE MONITORY
    
    # vyjmeme identifikaci udalosti z dane n-tice
    event_id = event[1]
    del(event[1])
    # priklad predani ke zpracovani udalosti moving
    if event_id == 'moving':
        # predame n-tici jako jednotlive parametry pri zachovani poradi
        onmoving(*event)
    elif event_id == 'requesting':
        onrequesting(*event)
    elif event_id == 'loading':
        onloading(*event)
    elif event_id == 'unloading':
        onunloading(*event)

###########################################################
# Nize netreba menit.

def monitor(reader):
    "Main function"
    for line in reader:
        line = line.strip()
        onevent(line.split())
    report_coverage()

if __name__ == "__main__":
    import sys
    monitor(sys.stdin)
