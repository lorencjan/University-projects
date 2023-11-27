# File: keyword_checker.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Contains class responsible for finding keywords for special keys.

from statistics import mean
from functools import reduce
from operator import xor
from itertools import chain

import constants
import layouts
from character_rules import char_rules
from key_checker_base import KeyCheckerBase


class KeywordChecker(KeyCheckerBase):
    """ Finds special key keywords in detected characters. """

    def __init__(self, rows, processed):
        """ Takes global detected rows and already processed characters and passes it to the parent class. """

        super().__init__(rows, processed)
        self.found_keywords = {}

    @staticmethod
    def check_partially_detected_modes(key, row, matched_characters):
        """ Modes generally consist of special characters which lead to possibility of false detection
            in character lines -> in case of partially detected 'mode', check if a skipped character is not
            detected elsewhere on the line -> it probably is not 'mode' then

            Parameters
            -------
            key : str
                Name of the key under test. Method does nothing if the key is not 'mode'
            row : list
                Current row of characters
            matched_characters : list
                Characters in the row matching the current character's rules

            Returns
            -------
            _ : bool
                True if everything is in order, False in case of the assumption of false detection
        """

        if key != constants.mode:
            return True

        skipped_chars = "".join([m["skipped"] for m in matched_characters if m["char"] is not None])
        row_chars = "".join([x["char"] for x in row])
        return not any(x in row_chars for x in skipped_chars)

    @staticmethod
    def get_spaces_between_keyword_chars(row, idx, width_interval, matched_characters):
        """ Converts the detection results to easier working format.

            Parameters
            -------
            row : list
                Current row of characters
            idx : int
                Index of currently tested character in the row
            width_interval : list
                Coordinates [x1, x2] specifying width of the keyword
            matched_characters : list
                Characters in the row matching the current character's rules

            Returns
            -------
            _ : list
                List space width values
        """

        char_intervals = [(row[idx]["x1"], row[idx]["x2"])] + \
                         [(m["char"]["x1"], m["char"]["x2"]) for m in matched_characters if m["char"] is not None]
        tmp = sorted((reduce(xor, map(set, chain([width_interval], char_intervals)))))
        space_intervals = [tmp[i:i + 2] for i in range(0, len(tmp), 2)]
        space_widths = [x[1] - x[0] for x in space_intervals]

        return space_widths

    def check_character_distances(self, row, idx, matched_characters, keyword_length, match_requirement):
        """ Checks distances between characters in a keyboard. There should be space for skipped (undetected)
            characters and also the characters should not be too far away from each other.
            This is done to avoid random matches throughout whole keyboard.

            Parameters
            -------
            row : list
                Current row of characters
            idx : int
                Index of currently tested character in the row
            matched_characters : list
                Characters in the row matching the current character's rules
            keyword_length : int
                Expected length of the current keyword
            match_requirement : int
                Minimum number of keyword characters required for it to be recognized as the target keyword

            Returns
            -------
            _ : bool
                True if all distances matches expectations, False otherwise
        """

        char_widths = [row[idx]["width"]] + [m["char"]["width"] for m in matched_characters if m["char"] is not None]
        mean_char_width = mean(char_widths)

        # tolerance for space between characters in a word ... it should be narrower then the char
        tolerance = max(mean_char_width // 5, 1)
        # defines how many keywords characters are missing (undetected) on the word borders / edges
        border_chars_skipped = sum(len(m["skipped"]) for m in matched_characters if m["char"] is None)
        # defines how many keywords characters are missing (undetected) inside the word
        inner_skipped = sum(len(m["skipped"]) for m in matched_characters if m["char"] is not None)

        # a matched character could be something far and falsely recognized, causing big width
        # -> check the spaces between characters and update
        to_remove = []
        border_indices = [i - 1 for i, m in enumerate(matched_characters) if i > 0 and m["char"] is None]
        for i, m in enumerate(matched_characters):
            max_space_between = (mean_char_width + tolerance) * len(m["skipped"]) + tolerance
            if "space_between" in m.keys() and max_space_between < m["space_between"]:
                to_remove.append(i)
                if i in border_indices:
                    border_chars_skipped += 1 + len(m["skipped"])
                    inner_skipped -= len(m["skipped"])
                else:
                    inner_skipped += 1

        for i in sorted(to_remove, reverse=True):
            del matched_characters[i]

        # if some distant incorrectly matched character was removed and the word has no longer enough characters, skip
        if len([x for x in matched_characters if x["char"] is not None]) + 1 < match_requirement:  # +1 is current char
            return False

        # now check if there is enough space for skipped characters
        x1, x2 = self.get_x_range_of_matched_characters(row, idx, matched_characters)
        space_widths = self.get_spaces_between_keyword_chars(row, idx, [x1, x2], matched_characters)
        if sum(space_widths) < tolerance * inner_skipped:
            return False

        # lastly, the characters can be just generally too far away from each other
        # -> skip if real width is bigger than expected maximum (prevents e.g. false 'mode' keywords in character lines)
        expected_length = keyword_length - border_chars_skipped
        if (tolerance + mean_char_width) * expected_length < x2 - x1:
            return False

        return True

    @staticmethod
    def check_keyword_line(recognized_chars):
        """ A lot of keywords are modes consisting of special chars. Those can be quite random (higher/lower) but
            still classified as same row thanks to other bigger chars in the row -> check y line again.

            Parameters
            -------
            recognized_chars : list
                Row characters recognized as part of keyword under test

            Returns
            -------
            _ : bool
                True if all chars are truly in the same y level == in one line.
        """

        for i, ch in enumerate(recognized_chars):
            if len(recognized_chars) > i + 1:
                nxt = recognized_chars[i + 1]
                y_diff = min(ch["y2"] - nxt["y1"], nxt["y2"] - ch["y1"])
                if y_diff < (ch["y2"] - ch["y1"]) / 2:
                    return False

        return True

    @staticmethod
    def check_tab_special_case(key, matched_characters, current_character):
        """ Special treatment for tab ... 't' must be detected because 'ab' sequence is very common.

            Parameters
            -------
            key : str
                Name of the key under test. Method does nothing if the key is not 'mode'
            matched_characters : list
                Characters in the row matching the current character's rules
            current_character : int
                Current character under test

            Returns
            -------
            _ : bool
                False if 't' is not detected in 'tab' keyword, True otherwise
        """

        if key != constants.tab:
            return True

        chars = [current_character["char"]] + [m["char"]["char"] for m in matched_characters if m["char"] is not None]
        return "t" in chars

    def check_mode_123_special_case(self, key, recognized_chars, row_idx):
        """ Mode containing 123 can be easily mistaken with number row and should be in lower half of the keyboard.

            Parameters
            -------
            key : str
                Name of the key under test. Method does nothing if the key is not 'mode'
            recognized_chars : list
                Row characters recognized as part of keyword under test
            row_idx : int
                Index of current row

            Returns
            -------
            _ : bool
                True mode with 123 is correctly placed.
        """

        if key != constants.mode:
            return True

        recognized_in_123_len = sum(1 if x["char"] in "123" else 0 for x in recognized_chars)
        return not (recognized_in_123_len > 1 and row_idx > len(self.rows) / 2)

    def check_if_found_better(self, key, recognized_chars):
        """ More of same keywords can be found. Replace old with new one if it's better.

            Parameters
            -------
            key : str
                Name of the key under test. Method does nothing if the key is not 'mode'
            recognized_chars : list
                Row characters recognized as part of keyword under test

            Returns
            -------
            _ : bool
                True if better than current has already been detected, False otherwise
        """

        if key not in self.found_keywords.keys():  # if it' is not already recognized, keep current
            return False

        found = self.found_keywords[key][-1]
        found_len = len(found)
        recognized_len = len(recognized_chars)
        has_more_confidence = sum(x["conf"] for x in found) > sum(x["conf"] for x in recognized_chars)
        if found_len > recognized_len or (found_len == recognized_len and has_more_confidence):
            return True

        return False

    def check_space_and_backspace_overlap(self, key, row, idx, x_coordinates):
        """ Space and backspace may overlap, check them and favor backspace

            Parameters
            -------
            key : str
                Name of the key under test. Method does nothing if the key is not 'mode'
            row : list
                Current row of characters
            idx : int
                Index of currently tested character in the row
            x_coordinates : tuple
                Coordinates (x1, x2) of current keyword

            Returns
            -------
            _ : bool
                True if better than current has already been detected, False otherwise
        """

        if key != constants.space or constants.backspace not in self.found_keywords.keys():
            return True

        _, _, backspace, _ = self.found_keywords[constants.backspace]
        backspace_bbox = [backspace["x1"], backspace["y1"], backspace["x2"], backspace["y2"]]
        space_bbox = [x_coordinates[0], row[idx]["y1"], x_coordinates[1], row[idx]["y2"]]

        return not self.rectangles_overlap(backspace_bbox, space_bbox)

    @staticmethod
    def accept_fully_detected_keyword_chars(matched_characters, recognized_chars):
        """ If there are all characters recognized in a keyword, they are considered processed.

            Parameters
            -------
            matched_characters : list
                Characters in the row matching the current character's rules
            recognized_chars : list
                Row characters recognized as part of keyword under test
        """

        if sum(len(m["skipped"]) for m in matched_characters) == 0:
            for ch in recognized_chars:
                ch["accepted"] = True

    def accept_previously_detected_worst_candidate(self, key):
        """ When we replace old matched keyword candidate with a new, better one, accept it
            even if it is not used in the end as it still is a recognized and processed keyword.

            Parameters
            -------
            key : str
                Name of the key under test. Method does nothing if the key is not 'mode'
        """

        if key in self.found_keywords.keys():
            _, _, _, duplicate_keyword = self.found_keywords[key]
            for ch in duplicate_keyword:
                ch["accepted"] = True

    def accept_found_keywords(self):
        """ Adds all found keywords to the processed keys list. """

        for row_idx, keyword_width_interval, char_to_add, _ in self.found_keywords.values():
            for ch in self.rows[row_idx]:
                if self.intervals_overlap(keyword_width_interval, (ch["x1"], ch["x2"])):
                    ch["accepted"] = True
            self.add_to_processed(char_to_add)

    def check_keywords(self):
        """ Runs the keyword checking process. It first matches a character and its neighbors with the character rules,
            then it checks distances between characters in the word and some special case. It keeps track of all
            detected potential keywords and for each special key it selects the best candidate.
        """

        keywords = constants.pages + constants.modes + constants.keywords + constants.alternative_keywords
        keywords_chars = set(str.join("", keywords))
        chars = [(i, j, x) for i, row in enumerate(self.rows) for j, x in enumerate(row) if x["char"] in keywords_chars]
        for i, j, c in chars:
            row = self.rows[i]
            for rule in char_rules[c["char"]][constants.special]:
                keyword = rule["left"] + c["char"] + rule["right"]
                if keyword not in keywords:
                    continue

                # firstly check if there are any matching rules for the character in the current row
                key = rule["key"]
                matched = self.find_character_rule_matches(i, j, rule)
                matched_len = len([x for x in matched if x["char"] is not None])
                match_requirement = round(len(keyword) * 2 / 3)  # at least 2 out of 3, 3 out of 4|5
                if (matched_len + 1 < match_requirement or  # +1 is current char
                        not self.check_partially_detected_modes(key, row, matched) or
                        not self.check_character_distances(row, j, matched, len(keyword), match_requirement)):
                    continue

                recognized_chars = [row[j], *[m["char"] for m in matched if m["char"] is not None]]

                # if it's 'abc'|'ABC' mode, it can be mode or shift ... rename for later use
                if key == constants.mode and sum(1 if x["char"] in "abc" else 0 for x in recognized_chars):
                    key = constants.mode_shift

                if (not self.check_keyword_line(recognized_chars) or
                        not self.check_tab_special_case(key, matched, row[j]) or
                        not self.check_mode_123_special_case(key, recognized_chars, i)):
                    continue

                self.accept_fully_detected_keyword_chars(matched, recognized_chars)
                x1, x2 = self.get_x_range_of_matched_characters(row, j, matched)
                if (self.check_if_found_better(key, recognized_chars) or
                        not self.check_space_and_backspace_overlap(key, row, j, (x1, x2))):
                    break

                self.accept_previously_detected_worst_candidate(key)
                to_add = {"char": key, "conf": -1, "x1": x1, "x2": x2, "y1": row[j]["y1"], "y2": row[j]["y2"]}
                self.found_keywords[key] = i, [x1, x2], to_add, recognized_chars
                break

        self.accept_found_keywords()
        self.remove_accepted()

    def correct_mode_shift_key(self, layout):
        """ Modes 'abc'|'ABC' might be both shift or a mode. For that reason those were temporarily classified
            as 'mode_shift'. If a keyboard layout is detected, it's converted to 'mode' or 'shift' depending if a
            'mode' or 'shift' has been already detected. Otherwise, just add it as a 'mode' as there is no 'shift' for
            special characters.

            Parameters
            -------
            layout : str
                Detected layout name
        """

        if constants.mode_shift not in self.processed.keys():
            return

        # check if it isn't among other chars to prevent just random detection in some word
        bbox = self.processed_first(constants.mode_shift)["bbox"]
        for ch in [ch for row in self.rows for ch in row if ch["char"] in layouts.alphabet]:
            tolerance = (ch["x2"] - ch["x1"]) // 2
            if self.rectangles_overlap(bbox, [ch["x1"] + tolerance, ch["y1"], ch["x2"] + tolerance, ch["y2"]]):
                del self.processed[constants.mode_shift]
                return

        # for existing layout add it if it doesn't already exist, ignore otherwise
        if layout in [constants.qwerty, constants.alphabet]:
            if constants.shift not in self.processed.keys():
                self.processed[constants.shift] = self.processed[constants.mode_shift]
            elif constants.mode not in self.processed.keys():
                self.processed[constants.mode] = self.processed[constants.mode_shift]
        else:  # for special char layout or other just add it as 'mode'
            if constants.mode in self.processed.keys():
                self.processed[constants.mode].append(self.processed_first(constants.mode_shift))
            else:
                self.processed[constants.mode] = self.processed[constants.mode_shift]

        del self.processed[constants.mode_shift]  # clean up
