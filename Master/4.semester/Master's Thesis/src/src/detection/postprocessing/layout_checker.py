# File: layout_checker.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Contains class responsible for finding keyboard's
#              alphabetical characters and recognizing keyboard layout.

from statistics import median

import layouts
import constants
from character_rules import char_rules
from key_checker_base import KeyCheckerBase


class LayoutChecker(KeyCheckerBase):
    """ Class responsible for finding keywords alphabetical characters and recognizing keyboard layout"""

    def __init__(self, rows, processed):
        """ Takes global detected rows and already processed characters and passes it to the parent class. """

        super().__init__(rows, processed)
        self.layout = None
        self.convert_to_upper = False

    def check_layout(self):
        """ For each layout discriminative character finds matches according to the character rules, selects the best
            according to which it sets the layout and computes missing characters. It also checks incorrect detections
            and corrects positions or capitalization as e.g. xX will surely be sometimes recognized wrong.

            Returns
            -------
            _ : str
                Name of the detected layout or None if no layout was detected.
        """

        matches = self.find_matches_for_discriminative_characters()
        if len(matches) < len(layouts.layout_discriminative_chars) // 4:  # at least 1/4 of characters must be found
            return None

        matches = self.select_best_candidates_in_matched_layout_rows(matches)
        matches = self.select_layout_and_its_matches(matches)
        self.process_matches(matches)
        # check if we truly added the layout chars, or it failed on some other validation
        self.layout = self.layout if any(k in layouts.alphabet for k in self.processed.keys()) else None
        self.check_layout_loners()
        self.remove_qwerty_false_positives()
        self.check_capitalization()
        self.remove_accepted()
        return self.layout

    def process_layout_char_rows(self, row_idx, char_idx, match_requirement=2):
        """ For each layout option of given discriminative character finds matches according to the character rules.

            Parameters
            -------
            row_idx : int
                Index of the row in which the currently tested character is
            char_idx : int
                Index of currently tested character in the row
            match_requirement : int
                Minimum number of characters that need to match the character rule for the sequence to be a candidate

            Returns
            -------
            _ : dict
                Matched characters for each layout option
        """

        matching_layouts = {}
        for layout_name, layout in char_rules[self.rows[row_idx][char_idx]["char"]][constants.layout].items():
            matched = self.find_character_rule_matches(row_idx, char_idx, layout)
            matched_len = len([x for x in matched if x["char"] is not None])
            if matched_len < match_requirement:
                continue

            # score is simple, number of matched minus number of skipper, only skipped have lower weight -> divided by 2
            score = matched_len - sum(len(m["skipped"]) for m in matched if m["char"] is not None) // 2
            matching_layouts[layout_name] = {"score": score, "results": (row_idx, char_idx, matched)}

        return matching_layouts

    def find_matches_for_discriminative_characters(self):
        """ For each layout discriminative character finds matches according to the character rules.

            Returns
            -------
            _ : dict
                Matched characters for each discriminative character for each layout option
        """

        chars = [(i, j, c["char"]) for i, row in enumerate(self.rows) for j, c in enumerate(row)
                 if c["char"] in layouts.layout_discriminative_chars]

        matches = {}
        for i, j, c in chars:
            result = self.process_layout_char_rows(i, j)
            if not result:
                continue

            if c in matches.keys():
                matches[c].append(result)
            else:
                matches[c] = [result]

        return matches

    @staticmethod
    def select_best_candidates_in_matched_layout_rows(matches):
        """ The characters can have several matches for a layout. This method selects the best ones,
            so that each character has only one candidate for each layout option.

            Parameters
            -------
            matches : dict
                Object with list of layout matches for each discriminative character

            Returns
            -------
            _ : dict
                Object with the best matches for each layout for each discriminative character
        """

        joined = {}
        for c, values in matches.items():
            for match in values:
                if c not in joined.keys():
                    joined[c] = match
                else:
                    for layout_name, results in match.items():
                        if layout_name not in joined[c].keys() or joined[c][layout_name]["score"] < results["score"]:
                            joined[c][layout_name] = results

        return joined

    def select_layout_and_its_matches(self, candidates):
        """ Selects the best layout based on given candidate matches.

            Parameters
            -------
            candidates : dict
                Object with the best matches for each layout for each discriminative character

            Returns
            -------
            _ : dict
                Candidates but just with the winning layout
        """

        if not candidates:
            return {}

        counter = {}
        for c, c_layouts in candidates.items():
            for layout_name, results in c_layouts.items():
                if layout_name not in counter.keys():
                    counter[layout_name] = results["score"]
                else:
                    counter[layout_name] += results["score"]

        self.layout = sorted(counter.items(), key=lambda x: x[1], reverse=True)[0][0]
        return {c: results for c, c_layouts in candidates.items() for layout_name, results in c_layouts.items() if
                layout_name == self.layout}

    def check_qwerty_row_order(self, discriminative_char, character):
        """ Checks that qwerty... row is the topmost row, asd... is in the middle and zxcv... at the bottom.
            The candidate match can still be incorrect, so leave it to the next one in this case.

            Parameters
            -------
            discriminative_char : str
                Current discriminative character which matches are being processed
            character : dict
                Currently processed matched character

            Returns
            -------
            _ : bool
                True if the row order is correct, False otherwise
        """

        if self.layout != constants.qwerty:
            return True
        elif discriminative_char in layouts.qwerty[0]:  # qwerty row should be above other rows
            ref = self.processed_first("a") if "a" in self.processed.keys() else self.processed_first("a")
            return ref is None or ref["bbox"][1] >= character["y1"]
        elif discriminative_char in layouts.qwerty[2]:  # bottom row should be below other rows
            ref = self.processed_first("a") if "a" in self.processed.keys() else self.processed_first("q")
            return ref is None or ref["bbox"][1] <= character["y1"]
        else:  # middle should be between the two other rows
            top_ref, bottom_ref = self.processed_first("q"), self.processed_first("x")
            return not ((bottom_ref and bottom_ref["bbox"][1] < character["y1"]) or
                        (top_ref and top_ref["bbox"][1] > character["y1"]))

    @staticmethod
    def correct_erroneous_o_vs_zero_detections(matched_characters):
        """ Corrects potential o vs 0 erroneous detection.

            Parameters
            -------
            matched_characters : list
                Characters in the row matching the current character's rules
        """

        for m in matched_characters:
            if m["char"] and m["char"]["char"] == "0":
                m["char"]["char"] = "o"

    @staticmethod
    def check_space_for_skipped_characters(matched_characters, character_width):
        """ To prevent random word detections, there must be space for skipped chars between matched chars.

            Parameters
            -------
            matched_characters : list
                Characters in the row matching the current character's rules
            character_width : int
                Width of the currently processed matched character
        """

        for m in matched_characters:
            if m["char"] and len(m["skipped"]) * character_width > max(m["neigh_dist"], 0):
                return False

        return True

    def process_matches(self, matches):
        """ Adds the best matching characters in the layout to the result.

            Parameters
            -------
            matches : dict
                Object with the best matched characters for winning layout for each discriminative character
        """

        processed_rows = []
        for c, results in sorted(matches.items(), key=lambda x: x[1]["score"], reverse=True):
            row_idx, char_idx, matched = results["results"]
            if row_idx in processed_rows or c in self.processed.keys():  # skip if already processed as each row has
                continue  # several discriminative characters

            row = self.rows[row_idx]
            if not self.check_qwerty_row_order(c, row[char_idx]):
                continue

            # in alphabetical layout it's unknown where the line ends -> cannot compute the rest -> remove borders
            if self.layout == constants.alphabet:
                matched = [x for x in matched if x["char"] is not None]

            if not self.check_space_for_skipped_characters(matched, row[char_idx]["width"]):
                continue

            self.correct_erroneous_o_vs_zero_detections(matched)
            self.process_char_in_row(row, char_idx, matched, layout=self.layout)
            self.remove_row_false_positives(row, char_idx, matched)
            processed_rows.append(row_idx)

    def process_character_row(self, row_idx, char_idx, match_requirement=0, default_x_distance=None):
        """ Directly adds matched characters (if found any) to the result without further validation.

            Parameters
            -------
            row_idx : int
                Index of the row in which the currently tested character is
            char_idx : int
                Index of currently tested character in the row
            match_requirement : int
                Minimum number of characters that need to match the character rule for the sequence to be a candidate
            default_x_distance : int
                Precomputed expected distance between characters in the row
        """

        row = self.rows[row_idx]
        rules = char_rules[row[char_idx]["char"]][constants.layout][self.layout]
        matched = self.find_character_rule_matches(row_idx, char_idx, rules)
        matched_len = len([x for x in matched if x["char"] is not None])
        if matched_len < match_requirement:
            return

        self.process_char_in_row(row, char_idx, matched, default_x_distance)
        self.remove_row_false_positives(row, char_idx, matched)

    def compute_layout_loner_row(self, bounding_rect, default_x_distance, y_type="y1"):
        """ Computes rest of a row for a detected character with not matches based on distance from other rows.

            Parameters
            -------
            bounding_rect : list
                Expected bounding box [x1, y1, x2, y2] for the missing row
            default_x_distance: int
                Expected distance between characters in the row
            y_type : str
                Defines which y value (y1|y2) should be used to check y coordinates
        """

        x1, y1, x2, y2 = bounding_rect
        for i, row in enumerate(self.rows):
            for j, c in enumerate(row):
                if not c["accepted"] and c["char"] not in self.processed.keys() and c["char"] in layouts.alphabet and \
                        x1 < c["x1"] < x2 and y1 < c[y_type] < y2:
                    self.process_character_row(i, j, 0, default_x_distance)
                    return  # even if there is more detected, this computes the rest of the row

    def check_layout_loners(self, recheck=True):
        """ It can happen that based on one or two rows a layout is selected, but a detected character
            from other row didn't have any matches, so it wasn't processed at all. Add this character
            for alphabet layout and compute its full row for qwerty layout.

            Parameters
            -------
            recheck : bool
                In case 2 rows are missing and we can compute one of them, it should be possible to compute
                the third one (qwerty). This parameter defines if the loner characters should be checked again.
        """

        if not self.layout:
            return

        keys = self.processed.keys()

        # alphabetical layout gives just order of characters, no way to determine the actual positions
        # as it's unknown where the lines are cut so that it can be computed -> keep what we found
        if self.layout == constants.alphabet:
            for row in self.rows:
                for idx in [i for i, x in enumerate(row) if not x["accepted"] and x["char"] in layouts.alphabet]:
                    if row[idx]["char"] not in keys:
                        self.add_to_processed(row[idx])
            return

        # we can compute qwerty

        def get_bbox(x):
            return self.processed_first(x)["bbox"]

        # if all rows are discovered, there's nothing to check
        if all(x in keys for x in ["q", "a", "x"]):
            return

        # bottom row is missing
        if all(x in keys for x in ["q", "a"]):
            x_diff = get_bbox("w")[0] - get_bbox("q")[0]
            y_diff = get_bbox("a")[1] - get_bbox("q")[1]
            y_min, y_max = get_bbox("a")[3], get_bbox("a")[3] + y_diff
            x_min, x_max = get_bbox("a")[0], get_bbox("l")[2]
            self.compute_layout_loner_row([x_min, y_min, x_max, y_max], x_diff)

        # middle row is missing
        elif all(x in keys for x in ["q", "x"]):
            x_diff = get_bbox("w")[0] - get_bbox("q")[0]
            y_min, y_max = get_bbox("q")[3], get_bbox("x")[1]
            x_min, x_max = get_bbox("q")[0], get_bbox("p")[2]
            self.compute_layout_loner_row([x_min, y_min, x_max, y_max], x_diff)

        # top row is missing
        elif all(x in keys for x in ["a", "x"]):
            x_diff = get_bbox("s")[0] - get_bbox("a")[0]
            y_diff = get_bbox("x")[1] - get_bbox("a")[1]
            y_min, y_max = get_bbox("a")[1] - y_diff, get_bbox("a")[1]
            x_min, x_max = get_bbox("a")[0] - x_diff, get_bbox("l")[2] + x_diff
            self.compute_layout_loner_row([x_min, y_min, x_max, y_max], x_diff, "y2")

        # if just one row is detected, we don't know the y differences between rows -> cannot confidently compute ranges
        # for all chars to compute row from (e.g. we have asd... row and we detected 'c', we don't know if it's 'c' from
        # row or 'c' from 'space' as x range is covered by both and y is quite close (other scenarios can be thought of)
        # -> instead of devising 100% bulletproof algorithm, checking just discriminative chars suffices
        else:
            default_x_dist = (get_bbox("w")[0] - get_bbox("q")[0] if "q" in keys else
                              get_bbox("s")[0] - get_bbox("a")[0] if "s" in keys else
                              get_bbox("c")[0] - get_bbox("x")[0])
            is_known_top, is_known_middle, is_known_bottom = "q" in keys, "a" in keys, "x" in keys
            for i, row in enumerate(self.rows):
                # compute row only if the character is unprocessed, discriminative and
                # correctly positioned relative to the only known row (below qwert..., above zxcv..., both for middle)
                for j in [j for j, x in enumerate(row)
                          if not x["accepted"] and x["char"] in layouts.layout_discriminative_chars and
                          ((is_known_top and x["y1"] > get_bbox("q")[3]) or
                           (is_known_middle and x["y1"] > get_bbox("a")[3] and x["char"] in layouts.qwerty[2]) or
                           (is_known_middle and x["y2"] < get_bbox("a")[1] and x["char"] in layouts.qwerty[0]) or
                           (is_known_bottom and x["y2"] < get_bbox("x")[1]))]:
                    self.process_character_row(i, j, 0, default_x_dist)
                    if recheck:  # a row is computed, so now we have 2 -> call again to compute the third
                        self.check_layout_loners(False)
                    return

    def remove_qwerty_false_positives_by_rows(self, layout_rect, x_padding=0, y_padding=0):
        """ As the qwerty row is the widest, this can include incorrectly some characters next to 'z' (e.g. shift)
            or special characters like comma or dot next to 'm', it's still worth checking it as full rectangle due to
            possible second key meaning row between layout rows etc. but the additional row check must be done.

            Parameters
            -------
            layout_rect : list
                Bounding box [x1, y1, x2, y2] for the qwerty layout
            x_padding : int
                Defines how much x padding to add then checking the width of potential false positives
            y_padding : int
                Defines how much y padding to add then checking the width of potential false positives
        """

        superfluous_chars = [(i, j, x) for i, row in enumerate(self.rows) for j, x in enumerate(row) if
                             not x["accepted"] and self.rectangles_overlap(layout_rect, self.to_bbox_list(x))]

        to_keep = []  # stores chars in qwerty bbox which are outside qwerty lines -> we don't remove these

        def find_chars_to_keep(min_x, max_x, min_y):
            return [(i, j) for i, j, x in superfluous_chars if
                    (x["y1"] >= min_y and (x["x2"] < min_x - x_padding or x["x1"] > max_x + x_padding)) or
                    # icons can have different sizes so check them separately
                    (x["char"] in constants.keywords and x["y1"] >= min_y and
                     not self.intervals_overlap((min_x, max_x), (x["x1"], x["x2"])))]

        keys = self.processed.keys()
        if "m" in keys:  # bottom row was recognized
            m, z = self.processed_first("m")["bbox"], self.processed_first("z")["bbox"]
            if not self.intervals_overlap((z[1], z[3]), (m[1], m[3])):  # handling qwert[y|z]
                z = self.processed_first("y")["bbox"]
            a = self.processed_first("a")["bbox"] if "a" in keys else None
            to_keep += find_chars_to_keep(z[0], m[2], a[3] + y_padding if a else m[1] - y_padding)

        if "a" in keys:  # middle row was recognized
            a, l = self.processed_first("a")["bbox"], self.processed_first("l")["bbox"]
            q = self.processed_first("q")["bbox"] if "q" in keys else None
            to_keep += find_chars_to_keep(a[0], l[2], q[3] + y_padding if q else a[1] - y_padding)

        # remove the rest
        to_keep = set(to_keep)
        for i, j, _ in sorted(superfluous_chars, key=lambda x: x[1], reverse=True):
            if (i, j) not in to_keep:
                del self.rows[i][j]

    def remove_layout_duplicates(self):
        """ There can be another duplicate (e.g. with diacritic) row. Keep the better row. """

        all_processed = [(k, ch) for k, arr in self.processed.items() for ch in arr]
        for c in "qax":  # for each row ... 'x' not to handle y|z
            if c not in self.processed.keys() or len(self.processed[c]) <= 1:
                continue

            # find rows among already processed characters
            rows = []
            for detection in self.processed[c]:
                bbox = detection["bbox"]
                row = filter(lambda x: self.intervals_overlap((bbox[1], bbox[3]), (x[1]["bbox"][1], x[1]["bbox"][3])),
                             all_processed)
                rows.append(list(row))

            # keep best -> remove all but first
            confidence_list = [(i, sum(x["conf"] for _, x in r)) for i, r in enumerate(rows)]
            for i, _ in sorted(confidence_list, key=lambda x: x[1], reverse=True)[1:]:
                for k, ch in rows[i]:
                    if len(self.processed[k]) > 1:
                        self.processed[k].remove(ch)

    def remove_qwerty_false_positives(self):
        """ Computes layout rectangle and removes any other characters in it - they don't belong there. """

        if self.layout is not constants.qwerty:
            return

        # layout rectangle coordinates
        x1_list, y1_list, x2_list, y2_list, widths, heights = [], [], [], [], [], []
        layout_bboxes = [x["bbox"] for key, values in self.processed.items() for x in values if key in layouts.alphabet]
        for bbox in layout_bboxes:
            x1_list.append(bbox[0]), y1_list.append(bbox[1]), x2_list.append(bbox[2]), y2_list.append(bbox[3])
            widths.append(bbox[2] - bbox[0]), heights.append(bbox[3] - bbox[1])

        # add padding to include potential special chars on the same key which are not valid though ... half of:
        # min for height -> min 'cause bbox can cover more than the char, min will always wrap tightest (most accurate)
        # median for width -> would go with min again, but chars like 'l' are too narrow for the padding to have effect
        x_padding, y_padding = median(widths) // 2, min(heights) // 2
        layout_rect = [min(x1_list) - x_padding, min(y1_list) - y_padding,
                       max(x2_list) + x_padding, max(y2_list) + y_padding]

        # remove detected which shouldn't be there
        self.remove_qwerty_false_positives_by_rows(layout_rect, x_padding, y_padding)
        self.remove_layout_duplicates()

    def check_capitalization(self):
        """ At the beginning, characters were converter to lowercase for easier manipulation.
            Information about its case was saved and this function decides if the characters
            should be converted to upper based on originally detected discriminative characters.
        """

        chars = [c for r in self.rows for c in r if c["accepted"] and c["char"] in layouts.capital_discriminative_chars]
        self.convert_to_upper = len([c for c in chars if c["isUpper"]]) > len(chars) // 2

    def correct_capitalization(self):
        """ Converts all processed characters to uppercase if it should. """

        if not self.convert_to_upper:
            return

        keys = [k for k in self.processed.keys() if k in layouts.alphabet]
        for k in keys:
            self.processed[str.upper(k)] = self.processed.pop(k)
