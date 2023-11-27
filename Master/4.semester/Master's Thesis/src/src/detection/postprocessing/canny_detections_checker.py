# File: canny_detections_checker.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Contains class responsible for checking detections of canny detector.
#              Those are used only for space and shift keys in qwerty layouts as
#              we are able to determine their probable locations.

from statistics import median

import constants
import layouts
from key_checker_base import KeyCheckerBase


class CannyDetectionsChecker(KeyCheckerBase):
    """ Class responsible for checking detections of canny detector. """

    def __init__(self, detections, processed):
        """ Takes canny detection bounding boxes and already processed characters and saves them. """

        super().__init__([], processed)
        self.detections = detections
        self.median_char_width = 0
        self.median_char_height = 0

    def filter_detections(self):
        """ We accept only shift left of bottom (zxcv...) row and space below the bottom row
             -> throw out any bbox not in left bottom part of the image (must be below asd... row and left of 'p' key)
             -> these keys are bigger then normal characters -> filter out anything smaller == noise
             -> same as too small we remove too big - higher than 1/2 of the qwerty layout (not width as space is wide)
        """

        chars = [val["bbox"] for ch, values in self.processed.items() for val in values if ch in layouts.alphabet]

        # filter bboxes outside expected region
        min_y = median(c[3] for c in chars)  # median should be from middle row under which we want the bboxes
        max_x = max(c[0] for c in chars)
        self.detections = [bbox for bbox in self.detections if  # centers of the bboxes must be within boundaries
                           (bbox[1] + bbox[3]) / 2 > min_y and (bbox[0] + bbox[2]) / 2 < max_x]

        # filter smaller bboxes than character bboxes
        self.median_char_width = median(c[2] - c[0] for c in chars)
        self.median_char_height = median(c[3] - c[1] for c in chars)
        self.detections = [bbox for bbox in self.detections if
                           bbox[2] - bbox[0] > self.median_char_width and bbox[3] - bbox[1] > self.median_char_height]

        # filter too high bboxes
        max_height = (max(c[3] for c in chars) - min(c[1] for c in chars)) / 2
        self.detections = [bbox for bbox in self.detections if bbox[3] - bbox[1] < max_height]

    def add_to_processed(self, character):
        """ Before actually creating a new key, a check whether a key already exists is necessary as this is
            just a supplementary process and doesn't take precedence over the neural network detection.
        """

        bbox = self.to_bbox_list(character)
        if any(self.rectangles_overlap(bbox, val["bbox"]) for values in self.processed.values() for val in values):
            return False

        super().add_to_processed(character)
        return True

    def guess_shift_key(self):
        """ Adds bounding box left from y|z in bottom row as shift key if it's at least twice the
            size of character bbox. If same, it can be some special character or whatever.
            We're not aiming at "undetected" shift icon (fault of the detector) but the key
            rectangle itself -> will not work if no key border exists.
        """

        if "m" not in self.processed.keys():  # check if bottom row is even detected
            return

        # handle y|z
        z, m = self.processed_first("z"), self.processed_first("m")
        if z is None or not self.intervals_overlap((z["bbox"][1], z["bbox"][3]), (m["bbox"][1], m["bbox"][3])):
            z = self.processed_first("y")

        median_char_area = self.median_char_width * self.median_char_height
        for bbox in self.detections:
            x1, y1, x2, y2 = bbox
            y_center = (y1 + y2) / 2
            bbox_area = (x2 - x1) * (y2 - y1)
            # if it is left and in the same row and is bigger than normal char -> save it
            if x2 < z["bbox"][0] and z["bbox"][1] < y_center < z["bbox"][3] and bbox_area > median_char_area * 2:
                if self.add_to_processed({"char": constants.shift, "conf": -1, "x1": x1, "x2": x2, "y1": y1, "y2": y2}):
                    return

    def guess_space_key(self):
        """ Adds bounding box below bottom row as a space if it is wider than usual. """

        if "m" not in self.processed.keys():  # check if bottom row is even detected
            return

        m = self.processed_first("m")["bbox"]
        min_x, max_x = self.processed_first("x")["bbox"][0], m[2]  # x not to handle y|z + it is enough
        for bbox in sorted(self.detections, key=lambda x: x[2] - x[0], reverse=True):
            x1, y1, x2, y2 = bbox
            x_center = (x1 + x2) / 2
            # if it is below bottom row (both y and x range) and is much wider than normal char -> save it
            if y1 > m[3] and min_x < x_center < max_x and (x2 - x1) > 5 * self.median_char_width:
                if self.add_to_processed({"char": constants.space, "conf": -1, "x1": x1, "x2": x2, "y1": y1, "y2": y2}):
                    return

    def guess_special_keys_from_bboxes(self):
        """ This method already expects we're operation on a qwerty layout. Based on that, it checks
            if there is wide bounding box below bottom row and classifies it as a space. Similarly,
            it classifies a bbox next to 'z' key on the left as a shift key.
        """

        # do nothing if both shift and space were successfully found (keyword or icon)
        has_shift = constants.shift in self.processed.keys()
        has_space = constants.space in self.processed.keys()
        if has_shift and has_space:
            return

        self.filter_detections()
        if not has_shift:
            self.guess_shift_key()

        if not has_space:
            self.guess_space_key()
