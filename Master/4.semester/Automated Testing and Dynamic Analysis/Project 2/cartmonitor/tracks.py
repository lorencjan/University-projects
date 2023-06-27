"""
Track implementation with path finder
"""

from collections import namedtuple

Track = namedtuple('Track', ['src', 'dst', 'cost'])

Pcost = namedtuple('PathCost', ['path', 'cost'])

def ucs(tracks, src, dst):
    "unified cost search for path finding"
    def min_idx(openset):
        minidx = 0
        i = 1
        while i < len(openset):
            if openset[i].cost < openset[minidx].cost:
                minidx = i
            i += 1
        return minidx

    # asserts
    if src == dst:
        return []
    if src not in tracks:
        return []
    if dst not in tracks:
        return []

    # initialization
    openset = []
    for track in tracks[src]:
        openset.append(Pcost([track], track.cost))

    # search
    while openset:
        # pop shortest path
        minidx = min_idx(openset)
        pc = openset[minidx]
        del openset[minidx]

        # are we finished?
        last = pc.path[-1].dst
        if last == dst:
            return pc.path
        # enroll it
        for track in tracks[last]:
            new_pc = Pcost(pc.path + [track], pc.cost + track.cost)
            openset.append(new_pc)

class Tracks:
    "mapped tracks"
    def __init__(self, tracks):
        # create track map: src -> (src, dst, delay)
        self.tracks = {}
        for track in tracks:
            if track.src in self.tracks:
                self.tracks[track.src].append(track)
            else:
                self.tracks[track.src] = [track]

    def get_path(self, src, dst):
        """
        Find shortest path from src to dst. Returns [] if invalid locations.
        Not connected subgraphs may cause infinite cycle.
        """
        return ucs(self.tracks, src, dst)

    def stations(self):
        "Return set of stations. Read only."
        return self.tracks.keys()
    
    def export(self, filename: str):
        "Exports map of tracks to Graphviz"
        with open(filename, "w") as output:
            output.write("digraph track {\n")
            for tracks in self.tracks.values():
                for track in tracks:
                    output.write('  %s -> %s [label="%s"];\n' % \
                        (track.src, track.dst, track.cost))
            output.write('}\n')

#MILL_TRACKS = [
#        Track('A', 'B', 20),
#        Track('B', 'C', 20),
#        Track('C', 'D', 20),
#        Track('D', 'A', 20),
#        Track('B', 'A', 30),
#        ]
#
#MILL = Tracks(MILL_TRACKS)
