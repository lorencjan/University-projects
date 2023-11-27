# File: key_checker_base.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Contains base class with common functionality for key checkers.

import abc
import operator
import statistics

import constants
import layouts


class KeyCheckerBase(metaclass=abc.ABCMeta):
    """ Base class for key checkers with common functionality. """

    def __init__(self, rows, processed):
        """ Takes global detected rows and already processed characters and keeps them. """

        self.rows = rows
        self.processed = processed

    @staticmethod
    def index_of(iterable, predicate, not_found=-1):
        """ Finds index in an iterable based on a predicate.

            Parameters
            -------
            iterable : iter
                Any iterable structure in which we search
            predicate : function
                Boolean predicate by which to find an item
            not_found : int
                Default index value if item not found

            Returns
            -------
            _ : int
                Index value of found item or default
        """

        return next(iter(i for i, x in enumerate(iterable) if predicate(x)), not_found)

    @staticmethod
    def compare_chars(expected, detected):
        """ Compares a character with character of detected object. Also takes into account possible o vs zero mistakes.

            Parameters
            -------
            expected : str
                Expected character as string
            detected : dict
                Object representing detected character

            Returns
            -------
            _ : bool
                True if the characters equal, False otherwise
        """

        equal = expected == detected["char"]
        if not equal:
            # detector can easily mistake zero and letter 'o' - we don't care, it's interchangeable
            if expected in "o0" and detected["char"] in "o0":
                equal = True

        return equal

    def check_expected_neighbors(self, row_idx, char_idx, expected_chars, is_left, layout=None):
        """ Checks current character and its row against expected characters from character rules.

            Parameters
            -------
            row_idx : int
                Index of the row in which the currently tested character is
            char_idx : int
                Index of currently tested character in the row
            expected_chars : str
                Expected characters from a rule in the direction from the current character (reversed for left side)
            is_left : bool
                Defines if left side from the current character is checked (or right one)
            layout : str
                Optional name of the checked layout which can help with row endings

            Returns
            -------
            _ : list
                List of matched characters in the row
        """

        row = self.rows[row_idx]
        op = operator.sub if is_left else operator.add
        detected_chars = list(reversed(row[:char_idx])) if is_left else row[char_idx + 1:]
        matched = []
        last_found_idx = -1
        for i, ch in enumerate(detected_chars):
            idx = self.index_of(expected_chars[last_found_idx + 1:], lambda x: self.compare_chars(x, ch))
            # skip if not found or found later than it was supposed to
            if idx == -1 or ch["char"] in [c for m in matched for c in m["skipped"]]:
                continue

            skipped = "".join(expected_chars[last_found_idx + 1:last_found_idx + idx + 1])
            # if we're skipping characters, check if there isn't another detected character
            # with higher confidence ... maybe it's just a too early false positive
            if layout and len(skipped) > 0:
                next_char = next(iter(c for c in detected_chars[i + 1:] if
                                      c["char"] == ch["char"] and c["conf"] > ch["conf"]), None)
                if next_char:
                    continue

            last = matched[-1]["char"] if len(matched) > 0 else row[char_idx]
            matched.append({
                "idx": op(char_idx, i + 1),
                "char": ch,
                "skipped": skipped,
                "neigh_dist": KeyCheckerBase.compute_neighbor_distance(ch, last),
                "space_between": max(0, last["x1"] - ch["x2"] if is_left else ch["x1"] - last["x2"])
            })
            last_found_idx += idx + 1

        # save also the remaining not detected (== matched) characters
        border_skipped = "".join(expected_chars[last_found_idx + 1:])
        matched.append({"idx": float("-inf" if is_left else "inf"), "char": None, "skipped": border_skipped})

        return matched

    @staticmethod
    def compute_neighbor_distance(bbox1, bbox2, axis="x", places_between=0):
        """ Computes distance between 2 detected object, either x or y.
            Takes into account possible skipped characters in between -> dividing the distance.

            Parameters
            -------
            bbox1 : dict
                First detected character object
            bbox2 : dict
                Second detected character object
            axis : str
                Defines if the computed distance should be on x or y axis
            places_between : int
                Defines how many characters should be between these two characters

            Returns
            -------
            _ : int
                Distance between centers of the bounding boxes of given characters
        """

        bbox1_center = (bbox1[f"{axis}1"] + bbox1[f"{axis}2"]) / 2
        bbox2_center = (bbox2[f"{axis}1"] + bbox2[f"{axis}2"]) / 2
        return round(abs(bbox1_center - bbox2_center) / (1 + places_between))

    @staticmethod
    def get_x_range_of_matched_characters(row, idx, matched_characters):
        """ Finds (x1, x2) span of current potential keyword under test.

            Parameters
            -------
            row : list
                Current row of characters
            idx : int
                Index of currently tested character in the row
            matched_characters : list
                Characters in the row matching the current character's rules

            Returns
            -------
            _ : tuple
                Coordinates (x1, x2) of current keyword
        """

        indices = [idx, *[m["idx"] for m in matched_characters if m["char"] is not None]]
        return row[min(indices)]["x1"], row[max(indices)]["x2"]

    @staticmethod
    def remove_row_false_positives(row, idx, matched_characters):
        """ Removes false positive detections == not accepted characters between inside a matched sequence.

            Parameters
            -------
            row : list
                Current row of characters
            idx : int
                Index of currently tested character in the row
            matched_characters : list
                Characters in the row matching the current character's rules
        """

        x_range = KeyCheckerBase.get_x_range_of_matched_characters(row, idx, matched_characters)
        for c in row:
            if not c["accepted"] and KeyCheckerBase.intervals_overlap((c["x1"], c["x2"]), x_range):
                row.remove(c)

    @staticmethod
    def rectangles_overlap(r1, r2):
        """ Takes two bounding boxes and checks if they overlap.

            Parameters
            -------
            r1 : list
                First rectangle in format [x1, y1, x2, y2]
            r2 : list
                Second rectangle in format [x1, y1, x2, y2]

            Returns
            -------
            _ : bool
                True if rectangles do overlap each other, False if not
        """

        return r1[0] < r2[2] and r1[2] > r2[0] and r1[3] > r2[1] and r1[1] < r2[3]

    @staticmethod
    def intervals_overlap(i1, i2):
        """ Takes two intervals and checks if they overlap.

            Parameters
            -------
            i1 : tuple
                First interval in format [x1, x2]
            i2 : tuple
                Second interval in format [x1, x2]

            Returns
            -------
            _ : bool
                True if intervals do overlap each other, False if not
        """

        return min(i1[1], i2[1]) > max(i1[0], i2[0])

    @staticmethod
    def to_bbox_list(obj):
        """ Converts detected character working object into a bounding box list.

            Parameters
            -------
            obj : dict
                Object representing detected character

            Returns
            -------
            _ : list
                Bounding box in format [x1, y1, x2, y2]
        """

        return [obj["x1"], obj["y1"], obj["x2"], obj["y2"]]

    def find_character_rule_matches(self, row_idx, char_idx, rules):
        """ Checks character neighbors with the expecting characters in a character rule.

            Parameters
            -------
            row_idx : int
                Index of the row in which the currently tested character is
            char_idx : int
                Index of currently tested character in the row
            rules : dict
                Character rule for a layout | keyword

            Returns
            -------
            _ : list
                List of rule matching characters in the row
        """

        left_expected, right_expected = "".join(reversed(rules["left"])), rules["right"]
        left_matched = self.check_expected_neighbors(row_idx, char_idx, left_expected, True)
        right_matched = self.check_expected_neighbors(row_idx, char_idx, right_expected, False)
        return left_matched + right_matched

    def process_matched_characters(self, row, idx, matched_characters, x_distance):
        """ Adds to processed matched characters while checking and correcting their position.

            Parameters
            -------
            row : list
                Current row of characters
            idx : int
                Index of currently tested character in the row
            matched_characters : list
                Characters in the row matching the current character's rules
            x_distance : int
                Precomputed expected distance between characters in the row
        """

        is_left = True
        next_position = 1
        # target can be i, j, l (much narrower) -> use the width as tolerance
        ref_width = row[idx]["x2"] - row[idx]["x1"]
        tolerance = ref_width // 2
        for m in matched_characters:

            if m["char"] is None:
                next_position = 1
                continue

            # traversing from matched chars on the left to the right
            if is_left and idx < m["idx"]:
                next_position = 1
                is_left = False

            # it can happen that a char is detected incorrectly as its neighbor (e.g i|j in alphabetical, j|l with
            # undetected 'k' in qwerty etc.) which results in computing missing character on incorrect position, so ...
            # if there is a skipped character and the detected is not in it's expected position (based on computed x
            # distance), move it to the expected position
            next_position += len(m["skipped"])
            char_range = (m["char"]["x1"], m["char"]["x2"])
            op = operator.sub if is_left else operator.add
            expected_x1 = op(row[idx]["x1"], next_position * x_distance)
            expected_x2 = op(row[idx]["x2"], next_position * x_distance)
            expected_range = (expected_x1 - tolerance, expected_x2 + tolerance)

            # adjust x position if not where expected
            if not self.intervals_overlap(char_range, expected_range):
                m["char"]["x1"] = expected_x1
                m["char"]["x2"] = expected_x2
                m["char"]["conf"] = -1

            self.add_to_processed(m["char"])
            next_position += 1

    def interchange_yz(self, character, bbox, layout):
        """ Checks positions of 'y' and 'z' characters to support qwertz layout.

            Parameters
            -------
            character : str
                Character under inspection
            bbox : list
                Expected bounding box coordinates [x1, y1, x2, y2] of the character
            layout : str
                Detected layout name

            Returns
            -------
            _ : bool
                True if x|y was processed, False otherwise
        """

        if layout != constants.qwerty:
            return False

        if character == "y":
            z = next(iter(x for row in self.rows for x in row if x["char"] == "z"), None)
            if z and self.rectangles_overlap(bbox, self.to_bbox_list(z)):
                self.add_to_processed(z)
                return True
        elif character == "z":
            y = next(iter(x for row in self.rows for x in row if x["char"] == "y"), None)
            if y and self.rectangles_overlap(bbox, self.to_bbox_list(y)):
                self.add_to_processed(y)
                return True

        return False

    def process_skipped_chars_in_row(self, row, idx, matched_characters, default_x_distance, layout=None):
        """ Computes positions of expected skipped characters and adds them to the result.

            Parameters
            -------
            row : list
                Current row of characters
            idx : int
                Index of currently tested character in the row
            matched_characters : list
                Characters in the row matching the current character's rules
            default_x_distance : int
                Optional precomputed expected distance between characters in the row
            layout : str
                Optional name of the checked layout which can help with row endings
        """

        is_left = True
        last_known = row[idx]
        for m in matched_characters:

            if is_left and idx < m["idx"]:
                last_known = row[idx]
                is_left = False

            if not m["skipped"]:
                last_known = m["char"]
                continue

            op = operator.sub if is_left else operator.add
            x_dist = (self.compute_neighbor_distance(last_known, m["char"], "x", len(m["skipped"]))
                      if m["char"] is not None else default_x_distance)
            y_dist = self.compute_neighbor_distance(last_known, m["char"], "y", 0) if m["char"] is not None else 0
            for i, ch in enumerate(m["skipped"]):
                x1 = op(last_known["x1"], (i + 1) * x_dist)
                x2 = op(last_known["x2"], (i + 1) * x_dist)
                y1 = last_known["y1"] + (y_dist if m["char"] and last_known["y1"] < m["char"]["y1"] else -y_dist)
                y2 = last_known["y2"] + (y_dist if m["char"] and last_known["y2"] < m["char"]["y2"] else -y_dist)

                if not self.interchange_yz(ch, [x1, y1, x2, y2], layout):
                    self.add_to_processed({"char": ch, "conf": -1, "x1": x1, "x2": x2, "y1": y1, "y2": y2})

            last_known = m["char"]

    def process_char_in_row(self, row, idx, matched_characters, default_x_distance=None, layout=None):
        """ Saves current character and its matches in the row.

            Parameters
            -------
            row : list
                Current row of characters
            idx : int
                Index of currently tested character in the row
            matched_characters : list
                Characters in the row matching the current character's rules
            default_x_distance : int
                Optional precomputed expected distance between characters in the row
            layout : str
                Optional name of the checked layout which can help with row endings

            Returns
            -------
            _ : int
                Expected distance between characters in the row
        """

        self.add_to_processed(row[idx])
        distances = [x["neigh_dist"] // (1 + len(x["skipped"])) for x in matched_characters if x["char"] is not None]
        x_distance = round(statistics.median(distances)) if distances else default_x_distance
        self.process_matched_characters(row, idx, matched_characters, x_distance)
        self.process_skipped_chars_in_row(row, idx, matched_characters, x_distance, layout)

        return x_distance

    def add_to_processed(self, character):
        """ Converts detected character in working format to output format and adds it to the processed dictionary.

            Parameters
            -------
            character : dict
                Processed character to save
        """

        character["accepted"] = True
        key = character["char"]
        obj = {"bbox": self.to_bbox_list(character), "conf": character["conf"]}
        if key in self.processed.keys():
            self.processed[key].append(obj)
        else:
            self.processed[key] = [obj]

    def processed_first(self, character):
        """ Gets the first (the only, but the format allows multiple == list) processed character data.

            Returns
            -------
            _ : dict
                Detection object already in output (also input) format with bbox and confidence
        """

        return self.processed[character][0] if character in self.processed.keys() else None

    def remove_accepted(self):
        """ Removes all accepted characters so that we don't compute with them again in the future. """

        for row in self.rows:
            indices = sorted([i for i, x in enumerate(row) if x["accepted"]], reverse=True)
            for i in indices:
                del row[i]
