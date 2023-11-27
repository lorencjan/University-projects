# File: keyword_checker.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Contains class responsible for checking and processing icons for special keys.

import constants
import layouts
from key_checker_base import KeyCheckerBase


class IconChecker(KeyCheckerBase):
    """ Class responsible for checking and processing icons for special keys. """

    def __init__(self, rows, processed, layout):
        """ Takes detected layout which it saves and global detected rows with
            already processed characters which are passed to the parent class.
        """

        super().__init__(rows, processed)
        self.layout = layout

    def save_best_and_remove_rest(self, key, candidates):
        """ Finds the icon with the highest confidence score and saves it to the result. Removes the rest.

            Parameters
            -------
            key : str
                Name of the special key of the icon
            candidates : list
                List of icon detection objects matching all validations from which to pick the most confident one.
        """

        if not candidates:
            return

        if key not in self.processed.keys():
            self.add_to_processed(candidates[0][2])
            candidates.pop(0)

        for i, j, _ in sorted(candidates, key=lambda x: x[1], reverse=True):
            del self.rows[i][j]

    def check_special_key_right_of_layout(self, key):
        """ Checks a key which is supposed to be on the right of main layout (qwerty) and saves it if it is.

            Parameters
            -------
            key : str
                Name of the special key being processed
        """

        # get the detected icons == special keys
        icons = sorted([(i, j, c) for i, row in enumerate(self.rows) for j, c in enumerate(row)
                       if c["char"] == key], key=lambda x: x[2]["conf"], reverse=True)

        # if not qwerty, just save the best (with the highest confidence score) icon
        if self.layout != constants.qwerty:
            self.save_best_and_remove_rest(key, icons)
            return

        # if qwerty, it should be right of layout (right of 'm' char)
        constraint = (self.processed_first("m") if "m" in self.processed.keys() else  # if not detected for any reason
                      self.processed_first("k") if "k" in self.processed.keys() else  # use 'k' which is above 'm'
                      self.processed_first("i"))                                      # or alternatively 'i'
        target = next(iter(x for _, _, x in icons if constraint["bbox"][2] < x["x1"]), None)
        if target:  # save only if there is any target icon right-side of the reference character
            self.add_to_processed(target)

    def check_shift(self):
        """ Checks positions of shift icons and add to result the more confident icon matching expected positions. """

        shifts = sorted([(i, j, c) for i, row in enumerate(self.rows) for j, c in enumerate(row)
                         if c["char"] == constants.shift], key=lambda x: x[2]["conf"], reverse=True)

        if len(shifts) == 0:
            return

        # shift should be next to or below the layout ... for alphabet layout just pick the best not above layout
        keys = self.processed.keys()
        if self.layout != constants.qwerty:
            if self.layout == constants.alphabet:
                first_alph_char = next(iter(self.processed_first(c)["bbox"] for c in layouts.alphabet if c in keys))
                shifts = [s for s in shifts if s[2]["y2"] > first_alph_char[1]]

            self.save_best_and_remove_rest(constants.shift, shifts)
            return

        # if qwerty, prefer the one next to 'z' ('x' not to check y|z)
        target = None
        if "x" in self.processed.keys():
            ref = self.processed_first("x")["bbox"]
            target = next(iter(s for _, _, s in shifts if self.intervals_overlap((ref[1], ref[3]), (s["y1"], s["y2"]))
                               and s["x2"] < ref[0]), None)

        # it doesn't need to be there necessarily, but it should be at least below 'asd...' line
        if not target:
            ref = (self.processed_first("s") if "s" in self.processed.keys() else self.processed_first("w"))["bbox"]
            target = next(iter(s for _, _, s in shifts if s["y1"] > ref[3]), None)
        # else on the left
        if not target:
            ref = self.processed_first("x")["bbox"]
            target = next(iter(s for _, _, s in shifts if s["x1"] < ref[0]), None)

        if target:
            self.add_to_processed(target)

    def check_tab(self):
        """ Checks positions of tab icons and adds to result only if we have qwerty layout
            and the icon is next to 'q'. Tab is not common on non-qwerty layouts and on
            qwerty it has just this position, so anything else is a false positive.
            It also retrospectively checks detected tab keyword for the same position.
        """

        tabs = [(i, j, c) for i, row in enumerate(self.rows) for j, c in enumerate(row) if c["char"] == constants.tab]
        tabs = sorted(tabs, key=lambda x: x[1], reverse=True)  # so that we can safely delete (going from last in rows)
        def get_bbox(x): return self.processed_first(x)["bbox"]

        # we accept only tab next to qwerty row in qwerty layout, none other as it's not a common special key otherwise
        x_boundary, y_min, y_max = None, None, None
        if "q" in self.processed.keys():
            q = get_bbox("q")
            x_boundary, y_min, y_max = q[0], q[1], q[3]
        if "a" in self.processed.keys():
            a = get_bbox("a")
            x_boundary = a[0]
            y_min = y_min if y_min else a[1]
            y_max = a[3]

        # the same check as for the icons should retrospectively be done on a found tab keyword
        tab = self.processed_first(constants.tab)
        if tab:
            bbox = tab["bbox"]
            if self.layout != constants.qwerty:
                del self.processed[constants.tab]  # must be qwerty as stated above
            elif bbox[2] > x_boundary or not self.intervals_overlap((y_min, y_max), (bbox[1], bbox[3])):
                del self.processed[constants.tab]  # must be next to qwerty row as stated above
            else:
                return  # if it's in the position, keep it and ignore icons as the words take precedence

        # again the same check as for tab keyword and select only the icon which matches the expected position
        for i, j, tab in tabs:
            if self.layout != constants.qwerty:
                del self.rows[i][j]
            elif tab["x2"] > x_boundary or not self.intervals_overlap((y_min, y_max), (tab["y1"], tab["y2"])):
                del self.rows[i][j]
            else:
                self.add_to_processed(tab)
                return

    def check_space(self):
        """ Checks positions of space icons and adds to result the best one. In non-qwerty layout the space can be
            anywhere, in qwerty layout it should be at the bottom half of the image, preferably below zxc... row.
            It also retrospectively checks detected space keyword for the same position.
        """

        spaces = sorted([(i, j, c) for i, row in enumerate(self.rows) for j, c in enumerate(row)
                         if c["char"] == constants.space], key=lambda x: x[2]["conf"], reverse=True)

        if len(spaces) == 0:
            return

        # space can be anywhere but bottom is preferred
        # -> if there's a detected space keyword and it's in the top half, prefer icon (retrospective keyword check)
        img_middle = max([val["bbox"][3] for list_values in self.processed.values() for val in list_values] or [0]) // 2
        space = self.processed_first(constants.space)
        if space is not None:
            if space["bbox"][3] < img_middle:
                del self.processed[constants.space]
            else:
                return  # prefer keyword to icon -> ignore the icons

        # in non-qwerty layout, space can be anywhere, just save the most confident one
        if self.layout != constants.qwerty:
            self.save_best_and_remove_rest(constants.space, spaces)
            return

        # in case of qwerty layout, space should be below the bottom row or at least in lower half of image
        if "m" in self.processed.keys():
            m = self.processed_first("m")["bbox"]
            min_x, max_x = self.processed_first("x")["bbox"][0], m[2]
            target = next(iter(x for x in spaces if min_x < x[2]["x1"] < max_x and x[2]["y2"] < m[1]), None)
            if target:
                self.add_to_processed(target[2])
            else:
                self.save_best_and_remove_rest(constants.space, [x for x in spaces if x[2]["y1"] > img_middle])

    def check_icons(self):
        """ Checks and adds to result icons for special keys (backspace, enter, shift, tab, space) """

        keys = self.processed.keys()

        if constants.backspace not in keys:
            self.check_special_key_right_of_layout(constants.backspace)

        if constants.enter not in keys:
            self.check_special_key_right_of_layout(constants.enter)

        if constants.shift not in keys:
            self.check_shift()

        self.check_tab()
        self.check_space()
        self.remove_accepted()
