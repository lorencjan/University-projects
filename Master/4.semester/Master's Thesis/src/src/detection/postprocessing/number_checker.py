# File: number_checker.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Contains class responsible for numbers and number patterns.

import operator

import layouts
import constants
from character_rules import char_rules
from key_checker_base import KeyCheckerBase


class NumberChecker(KeyCheckerBase):
    """ Class responsible for numbers and number patterns. """

    def __init__(self, rows, processed):
        """ Takes global detected rows and already processed characters and passes it to the parent class. """

        super().__init__(rows, processed)

    def get_numbers(self):
        """ Helper function finding numbers among detected characters.

            Returns
            -------
            _ : list
                List of detected numbers with their indices in the rows grid
        """

        return [(i, j, x) for i, row in enumerate(self.rows) for j, x in enumerate(row) if x["char"] in "123456789"]

    @staticmethod
    def check_pinpad_column(numbers, number, detected_column, expected_column):
        """ If all 3 numbers in a column aare detected, it's valid. However, the column doesn't
            need to be complete (can be computed) but some other number in the missing row must
            exist to prevent random overlaps == false pin-pad column detections.

            Parameters
            -------
            numbers : list
                List of detected numbers
            number : dict
                Number character object which is currently being processed
            detected_column : list
                Detected numbers forming a potential pin-pad column
            expected_column : str
                Character rule for pin-pad column for current number under test

            Returns
            -------
            _ : bool
                True if the detected column is valid, False otherwise
        """

        if len(detected_column) == 2:  # if all 3 (2 detected + 1 current) were detected -> ok
            return True

        found = detected_column[0]
        missing = str.replace(expected_column, found["char"], "")
        if missing in "456":
            middle_y = (number["y1"] - found["y2"] if number["y1"] > found["y1"] else found["y1"] - number["y2"]) // 2
        else:
            y_diff = abs(number["y1"] - found["y1"]) - number["height"] // 2
            middle, other = (number, found) if number["char"] in "456" else (found, number)
            middle_y = middle["y1"] - y_diff if middle["y1"] < other["y2"] else middle["y2"] + y_diff

        return any(n["y1"] < middle_y < n["y2"] for _, _, n in numbers)

    def process_number_in_column(self, number, detected_column):
        """ Saves column numbers to the result and computes y distance to be able to compute the rest.

            Parameters
            -------
            number : dict
                Number character object which is currently being processed
            detected_column : list
                Detected numbers forming a potential pin-pad column

            Returns
            -------
            _ : int
                Y distance between the numbers in the column
        """

        y_distance = None
        for detected_numbed in detected_column:
            if not y_distance:
                pair = (number, detected_numbed)
                space_between = 0 if any(x["char"] in "456" for x in pair) else 1
                y_distance = self.compute_neighbor_distance(*pair, "y", space_between)

            self.add_to_processed(detected_numbed)

        return y_distance

    def check_pinpad_layout(self, x_distance):
        """ Checks every character in each row and either accepts the detected ones or computes expected.

            Parameters
            -------
            x_distance : int
                Precomputed expected x distance between numbers in a pin-pad row

            Returns
            -------
            _ : str
                Missing expected row
        """

        skipped_row = None
        numbers = [n for row in self.rows for n in row if not n["accepted"] and n["char"] in layouts.numbers]
        for expected_row in layouts.pinpad_rows:
            found = next(iter((i, self.processed_first(c)["bbox"]) for i, c in enumerate(expected_row)
                              if c in self.processed.keys()), None)

            if not found:                   # this can happen only once as the previous validations
                skipped_row = expected_row  # allow only 1 missing pin-pad row
                continue

            # compute expected position for each remaining number ... if detected -> add, else create the computed
            idx, number = found
            for i, expected in [(i, c) for i, c in enumerate(expected_row) if c not in self.processed.keys()]:
                x_diff = (i - idx) * x_distance
                expected_bbox = [number[0] + x_diff, number[1], number[2] + x_diff, number[3]]
                to_add = [n for n in numbers if self.rectangles_overlap(expected_bbox, self.to_bbox_list(n))]
                if to_add:
                    self.add_to_processed(to_add[0])
                else:
                    self.add_to_processed({
                        "char": expected, "conf": -1,
                        "x1": expected_bbox[0], "x2": expected_bbox[2],
                        "y1": expected_bbox[1], "y2": expected_bbox[3]})

        return skipped_row

    def process_skipped_pinpad_row(self, skipped_row, y_distance):
        """ Finds which row is missing and computes coordinates its numbers.

            Parameters
            -------
            skipped_row : str
                Expected undetected == skipped row
            y_distance : int
                Precomputed expected y distance between numbers in the pin-pad column
        """

        if not skipped_row:
            return

        # skipped_row[0] is one of 1,4,7 (first pin-pad column)
        column_rule = char_rules[skipped_row[0]][constants.pinpad_column]

        def get_bbox(x): return self.processed_first(x)["bbox"]

        if column_rule == "17":  # missing middle row
            missing_row = "456"
            x_ref = [get_bbox(x) for x in "123"]
            y1 = get_bbox("1")[1] + get_bbox("7")[1] // 2
            y2 = get_bbox("1")[3] + get_bbox("7")[3] // 2
        else:
            missing_row = "789" if column_rule == "14" else "123"  # both can be either top or bottom
            x_ref = [get_bbox(x) for x in "456"]
            # subtract (add) y distance from (to) middle row depending which one (123|789) is top or bottom
            op = operator.sub if get_bbox("4")[1] < get_bbox("7" if missing_row[0] == "1" else "1")[1] else operator.add
            y1, y2 = op(get_bbox("4")[1], y_distance), op(get_bbox("4")[3], y_distance)

        # compute the row
        for i, n in enumerate(missing_row):
            obj = {"char": n, "conf": -1, "x1": x_ref[i][0], "x2": x_ref[i][2], "y1": y1, "y2": y2}
            self.add_to_processed(obj)

    def check_zero_position(self, target, y_distance):
        """ Checks if a character (zero) is under pin-pad bottom row (either 123 or 789, both can be bottom).

            Parameters
            -------
            target : dict
                Detection object (zero or incorrect 'o') to test
            y_distance : int
                Precomputed expected y distance between numbers in the pin-pad column

            Returns
            -------
            _ : bool
                True is in position, False otherwise
        """

        two, seven = self.processed_first("2")["bbox"], self.processed_first("7")["bbox"]
        ref_y = two[1] if two[1] < seven[3] else seven[1]
        range_x = (seven[0], self.processed_first("9")["bbox"][2])

        return (ref_y - y_distance) < target["y2"] and self.intervals_overlap((target["x1"], target["x2"]), range_x)

    def check_pinpad_zero(self, y_distance):
        """ Checks zero position under bottom pinpad row and also handles potential 'o' vs zero error.

            Parameters
            -------
            y_distance : int
                Precomputed expected y distance between numbers in the pin-pad column
        """

        zero = next(iter(x for row in self.rows for x in row if x["char"] == "0"), None)
        if zero and self.check_zero_position(zero, y_distance):
            self.add_to_processed(zero)
        else:
            detections_o_O = [x for row in self.rows for x in row
                              if not x["accepted"] and (x["char"] == "o" or x["char"] == "O")]
            wrong_o_O_as_zero = next(iter(x for x in detections_o_O if self.check_zero_position(x, y_distance)), None)
            if wrong_o_O_as_zero:
                wrong_o_O_as_zero["char"] = "0"
                self.add_to_processed(wrong_o_O_as_zero)

    def remove_pinpad_false_positives(self):
        """ Finds and removes all characters among pin-pad numbers which should not be there. """

        def get_bbox(x):
            return self.processed_first(x)["bbox"]

        # find the pin-pad rectangle
        four, seven = get_bbox("4"), get_bbox("7")
        x1, x2 = four[0], get_bbox("6")[2]
        y1, y2 = min(get_bbox("1")[1], seven[1]), max(get_bbox("1")[3], seven[3])

        # create some padding (half of space between the numbers) to cover alphabetical chars under/next to the numbers
        x_padding = (get_bbox("5")[0] - four[2]) // 2
        y_padding = (seven[1] - four[3] if seven[1] > four[1] else four[3] - seven[1]) // 2

        # remove all unprocessed characters inside the pin-pad bounding box
        # ... also create special bbox for 0 as it's outside the pin-pad bbox
        ref_rect = [x1 - x_padding, y1 - y_padding, x2 + x_padding, y2 + y_padding]
        for row in self.rows:
            zero_x1, zero_y1, zero_x2, zero_y2 = get_bbox("0") if "0" in self.processed.keys() else [0, 0, 0, 0]
            zero_bbox = [zero_x1 - x_padding, zero_y1 - y_padding, zero_x2 + x_padding, zero_y2 + y_padding]
            indices = [i for i, c in enumerate(row) if not c["accepted"] and
                       (self.rectangles_overlap(ref_rect, self.to_bbox_list(c)) or
                        self.rectangles_overlap(zero_bbox, self.to_bbox_list(c)))]
            for i in sorted(indices, reverse=True):
                del row[i]

    def check_pinpad(self):
        """ Checks if there is a pin-pad layout pattern among the detected characters and processes it. """

        numbers = self.get_numbers()
        if len(numbers) == 0:
            return

        for i, j, num in numbers:
            row = self.rows[i]

            # check column
            column_rule = char_rules[num["char"]][constants.pinpad_column]
            detected_col = [n for ii, jj, n in numbers if (ii != i or jj != j) and n["char"] in column_rule
                            and self.intervals_overlap((n["x1"], n["x2"]), (num["x1"], num["x2"]))]

            if len(detected_col) == 0 or not self.check_pinpad_column(numbers, num, detected_col, column_rule):
                continue

            # check row
            matched = self.find_character_rule_matches(i, j, char_rules[num["char"]][constants.pinpad])
            matched_len = len([x for x in matched if x["char"] is not None])
            if matched_len == 0:
                continue

            # compute the rest
            x_distance = self.process_char_in_row(row, j, matched)
            y_distance = self.process_number_in_column(num, detected_col)
            skipped_row = self.check_pinpad_layout(x_distance)
            self.process_skipped_pinpad_row(skipped_row, y_distance)
            self.check_pinpad_zero(y_distance)
            self.remove_pinpad_false_positives()
            break

        self.remove_accepted()

    @staticmethod
    def remove_skipped_zero(matched_characters):
        """ Remove 0 from number line border skipped as it's unknown if it should be left or right.

            Parameters
            -------
            matched_characters : list
                Characters in the row matching the current character's rules
        """

        left_border, right_border = [m for m in matched_characters if m["char"] is None]
        left_border["skipped"] = left_border["skipped"].replace("0", "")
        right_border["skipped"] = right_border["skipped"].replace("0", "")

    @staticmethod
    def check_incomplete_number_line(layout, matched_characters, row, char_idx):
        """ If not qwerty, row could be cut -> remove skipped borders if a border char is not recognized. If the number
            line is cut == incomplete, the border skipped numbers are not computed as we don't kno where it ends.

            Parameters
            -------
            layout : str
                Name of the detected keyboard layout
            matched_characters : list
                Characters in the row matching the current character's rules
            row : list
                Current row of characters
            char_idx : int
                Index of currently tested character in the row

            Returns
            -------
            _ : bool
                True if the number row is cut == incomplete, False if full
        """

        if layout == constants.qwerty:
            return False

        # get the left and right-most numbers in the line and the borders with skipped chars
        indices = [char_idx] + [m["idx"] for m in matched_characters if m["char"] is not None]
        left_most, right_most = row[min(indices)], row[max(indices)]
        left_border, right_border = [m for m in matched_characters if m["char"] is None]

        # it's highly unlikely the layout would cut the number row for last <= 3 numbers (counting zeros)
        row_is_cut = False
        if left_most["char"] not in "0123":
            matched_characters.remove(left_border)
            row_is_cut = True
        if right_most["char"] not in "7890":
            matched_characters.remove(right_border)
            row_is_cut = True

        return row_is_cut

    @staticmethod
    def correct_erroneous_o_vs_zero_detections(matched_characters):
        """ Corrects potential o vs 0 erroneous detection.

            Parameters
            -------
            matched_characters : list
                Characters in the row matching the current character's rules
        """

        for m in matched_characters:
            if m["char"] and m["char"]["char"] == "o":
                m["char"]["char"] = "0"

    def check_number_line_position_in_qwerty(self, layout, number):
        """ If above qwerty, it can be on the qwerty keys (e.g. phones) -> check row distance.

            Parameters
            -------
            layout : str
                Name of the detected keyboard layout
            number : dict
                Number character object which is currently being processed

            Returns
            -------
            _ : bool
                True if the number row is valid, False otherwise
        """

        if layout != constants.qwerty or "q" not in self.processed.keys():
            return True

        # if the numbers are on qwerty... row keys it's 1 on q, 2 on w etc.
        pairs = {str((i + 1) % 10): c for i, c in enumerate(layouts.qwerty[0])}

        # compute the expected x and y tolerances
        q = self.processed_first("q")["bbox"]
        x_diff = (self.processed_first("w")["bbox"][0] - q[2]) // 2  # half of space between
        ref = (self.processed_first("a") if "a" in self.processed.keys() else self.processed_first("x"))["bbox"]
        y_diff = ref[1] - q[3]                # space between
        if "a" not in self.processed.keys():  # handle missing middle row
            y_diff -= q[3] - q[1]             # remove height of middle row
            y_diff //= 2                       # we want just 1 space between

        # invalidate if the number is not just too close to the qwerty row but also to its paired character
        # because the row can just be shifted but still correct (just y distance is not enough)
        if q[1] - y_diff < number["y1"]:  # y is too close
            ch = self.processed_first(pairs[number["char"]])["bbox"]

            # handle y|z problem
            if pairs[number["char"]] == "y" and not self.intervals_overlap((ch[1], ch[3]), (q[1], q[3])):
                if "z" in self.processed.keys():
                    ch = self.processed_first("z")["bbox"]
                else:
                    return False  # ignore if it cannot be checked

            # invalidate if not just y, but also x is too close
            if self.intervals_overlap((number["x1"], number["x2"]), (ch[0] - x_diff, ch[2] + x_diff)):
                return False

        return True

    def check_number_line(self, layout, match_requirement=3):
        """ Checks and processes number line sequence [0]123456789[0].

            Parameters
            -------
            layout : str
                Name of the detected keyboard layout
            match_requirement : int
                Minimum number of numbers required for it to be recognized as a number row
        """

        numbers = self.get_numbers()
        if len(numbers) == 0:
            return

        for i, j, num in numbers:
            if num["accepted"]:
                continue

            # find rule matching row
            row = self.rows[i]
            matched = self.find_character_rule_matches(i, j, char_rules[num["char"]][constants.number_line])
            matched_len = len([x for x in matched if x["char"] is not None])
            if matched_len < match_requirement:
                continue

            # check row features and position
            self.remove_skipped_zero(matched)
            row_is_cut = self.check_incomplete_number_line(layout, matched, row, j)
            self.correct_erroneous_o_vs_zero_detections(matched)
            if not self.check_number_line_position_in_qwerty(layout, num):
                continue

            # save the row
            self.process_char_in_row(row, j, matched)
            self.remove_row_false_positives(row, j, matched)
            if not row_is_cut:  # if incomplete, have another run to save the rest
                break           # otherwise end as the row is processed

        self.remove_accepted()
