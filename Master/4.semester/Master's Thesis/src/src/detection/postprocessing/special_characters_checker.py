# File: special_characters_checker.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Contains class responsible for checking and processing of special characters.

import operator
from statistics import median

import constants
import layouts
from character_rules import char_rules
from key_checker_base import KeyCheckerBase


class SpecialCharactersChecker(KeyCheckerBase):
    """ Class responsible for checking and processing of special characters. """

    def __init__(self, rows, processed, layout):
        """ Takes detected layout which it saves and global detected rows with
            already processed characters which are passed to the parent class.
        """

        super().__init__(rows, processed)
        self.layout = layout
        self.char_line_detected = False

    def get_chars(self):
        """ Helper function finding special characters among detected characters.

            Returns
            -------
            _ : list
                List of detected special characters with their indices in the rows grid
        """

        return [(i, j, x) for i, row in enumerate(self.rows) for j, x in enumerate(row)
                if not x["accepted"] and x["char"] in layouts.special_chars]

    def get_top_qwerty_row(self):
        """ Finds left-most and right-most bboxes of the detected top qwerty layout row.

            Returns
            -------
            _ : tuple
                Left-most and right-most bboxes of the detected top qwerty layout row
        """

        def get_bbox(x): return self.processed_first(x)["bbox"]
        return (get_bbox("q"), get_bbox("p")) if "q" in self.processed.keys() else\
               (get_bbox("a"), get_bbox("l")) if "a" in self.processed.keys() else\
               (get_bbox("x"), get_bbox("m"))

    def is_qwerty_with_number_line_above(self):
        """ Character line detection should be ignored if we have qwerty layout with detected number line above it.
            In that case the characters from character line is I daresay always on the number keys.

            Returns
            -------
            _ : bool
                True if it is qwerty layout and number line was detected above it, False otherwise
        """

        if self.layout != constants.qwerty or "5" not in self.processed.keys():
            return False

        left_bbox, right_bbox = self.get_top_qwerty_row()
        fives = self.processed["5"]
        return any(f["bbox"][1] < left_bbox[1] and left_bbox[0] < f["bbox"][0] < right_bbox[0] for f in fives)

    def check_char_line_distances(self, row_idx, char_idx, matched_characters):
        """ Validates expected distance between characters in special character line.

            Parameters
            -------
            row_idx : int
                Index of the row in which the currently tested character is
            char_idx : int
                Index of currently tested character in the row
            matched_characters : list
                Characters in the row matching the current character's rules

            Returns
            -------
            _ : bool
                True if all characters are on the expected positions, False otherwise.
        """

        # if there are no immediate neighbors detected, ignore the potential char line
        neighbor_distances = [m["neigh_dist"] for m in matched_characters if m["char"] and len(m["skipped"]) == 0]
        if len(neighbor_distances) == 0:
            return False

        # check the distances
        character = self.rows[row_idx][char_idx]
        x_distance = round(median(neighbor_distances))
        next_position = 1
        for m in matched_characters:
            if m["char"] is None:
                continue

            op = operator.add if m["idx"] > char_idx else operator.sub
            next_position += len(m["skipped"])
            distance = next_position * x_distance
            expected_x1, expected_x2 = op(character["x1"], distance), op(character["x2"], distance)
            if not self.intervals_overlap((expected_x1, expected_x2), (m["char"]["x1"], m["char"]["x2"])):
                return False

            next_position += 1

        return True

    def check_char_line(self, match_requirement=3):
        """ Finds common special character line !@#$%^&*() and processes it.

            Parameters
            -------
            match_requirement : int
                Minimum number of characters that need to match the character rule for the sequence to be a candidate
        """

        if self.is_qwerty_with_number_line_above():
            return

        for i, j, c in self.get_chars():
            if c["char"] not in layouts.special_char_line:
                continue

            matched = self.find_character_rule_matches(i, j, char_rules[c["char"]][constants.char_line])
            matched_len = len([x for x in matched if x["char"] is not None])
            if matched_len < match_requirement or not self.check_char_line_distances(i, j, matched):
                continue

            self.process_char_in_row(self.rows[i], j, matched)
            self.char_line_detected = True
            return

    def remove_special_chars_above_qwerty(self):
        """ In case of qwerty layout, there are no special character above the layout -> we can remove false positives.
            Above qwerty is usually either nothing or a number or special char row. Special character row is already
            processed by now, so we remove anythin above qwerty... row or number row if there is any.
        """

        if self.layout != constants.qwerty:
            return

        # first detect numbers above qwerty row
        left_qwerty_bbox, right_qwerty_bbox = self.get_top_qwerty_row()
        is_num_row = False
        num_row_x1_list, num_row_y1_list, num_row_x2_list, num_row_y2_list = [], [], [], []
        for num_key in [k for k in self.processed.keys() if k in layouts.numbers]:
            bbox = self.processed_first(num_key)["bbox"]
            if left_qwerty_bbox[0] < bbox[0] < right_qwerty_bbox[0] and bbox[1] < left_qwerty_bbox[1]:
                is_num_row = True
                num_row_x1_list.append(bbox[0])
                num_row_y1_list.append(bbox[1])
                num_row_x2_list.append(bbox[2])
                num_row_y2_list.append(bbox[3])

        # if there are numbers, remove characters above number row
        to_remove = []
        chars = self.get_chars()
        if is_num_row:
            x1, y1, x2, y2 = min(num_row_x1_list), min(num_row_y1_list), max(num_row_x2_list), max(num_row_y2_list)
            for i, j, c in chars:
                if c["y2"] < (y1 + y2) / 2:
                    to_remove.append((i, j))  # remove all above the center of number row as they may overlap a bit
                elif self.rectangles_overlap([x1, y1, x2, y2], [c["x1"], c["y1"], c["x2"], c["y2"]]):
                    to_remove.append((i, j))  # remove all between the numbers in the number row

        # if number line is not detected, remove characters above qwerty... row or special character row
        elif self.char_line_detected or "q" in self.processed.keys():
            bbox = self.processed_first("%" if self.char_line_detected else "q")["bbox"]
            for i, j, c in chars:
                if c["y2"] < bbox[1]:
                    to_remove.append((i, j))

        for i, j in sorted(to_remove, key=lambda x: x[1], reverse=True):
            del self.rows[i][j]

    def process_remaining_special_chars(self):
        """ By now everything important has been already checked. From the remaining characters
            select the most confident ones and save these as we have no information about them.
        """

        for i, j, c in self.get_chars():
            self.add_to_processed(c)

        # keep just most confident ones from duplicates
        for c in layouts.special_chars:
            if c in self.processed.keys():
                self.processed[c] = sorted(self.processed[c], key=lambda x: x["conf"], reverse=True)[:1]

    def check_special_chars(self):
        """ Processes and saves detected special characters. """

        self.check_char_line()
        self.remove_special_chars_above_qwerty()
        self.process_remaining_special_chars()
        self.remove_accepted()
