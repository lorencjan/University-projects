# File: post_processor.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Runs corrections on detection results. Some characters might be undetected/incorrectly detected etc. and
#              due to more or less consistent keyboard layouts those can be checked (e.g. computed missing characters)

import os
import argparse
import json
import cv2
from itertools import takewhile

import layouts
import constants
from layout_checker import LayoutChecker
from icon_checker import IconChecker
from keyword_checker import KeywordChecker
from number_checker import NumberChecker
from special_characters_checker import SpecialCharactersChecker
from canny_detections_checker import CannyDetectionsChecker


class PostProcessor:
    """ Namespace class providing character detection postprocessing functionality
        using various key checkers in order to check and correct detection results.
    """

    @staticmethod
    def convert_detections(detections):
        """ Converts the detection results to easier working format.

            Parameters
            -------
            detections : dict
                Dict/JSON like detection results

            Returns
            -------
            _ : list
                List of all detections in a working format
        """

        converted = []
        for character, bbox_list in detections.items():
            for bbox in bbox_list:
                x1, y1, x2, y2 = bbox["bbox"]
                converted.append({
                    "char": str.lower(character),       # it's easier to work with unified case
                    "isUpper": str.isupper(character),
                    "x1": x1, "y1": y1, "x2": x2, "y2": y2,
                    "width": x2 - x1,
                    "height": y2 - y1,
                    "conf": bbox["conf"],
                    "accepted": False
                })

        return converted

    @staticmethod
    def find_rows(detections):
        """ Finds characters which are in a row.

            Parameters
            -------
            detections : dict
                Dict/JSON like detection results

            Returns
            -------
            _ : list
                Detected characters ordered in rows forming a grid structure
        """

        # sorting ensures going from bottom to top (coordinates starts in top left, so max y is bottom)
        detections = sorted(PostProcessor.convert_detections(detections), key=lambda x: x["y2"], reverse=True)
        rows = []
        while detections:
            # to be in the same row, they must overlap over 50% of the height
            row = list(takewhile(lambda x: x["y2"] - detections[0]["y1"] >= 0.5 * (x["height"]), detections))
            row = sorted(row, key=lambda x: x["x1"])  # ensures left to right order
            rows.append(row)
            detections = detections[len(row):]

        return rows

    @staticmethod
    def run(character_detections, canny_detections=None):
        """ Runs checks for keywords, layouts, special characters etc.

            Parameters
            -------
            character_detections : dict
                Detections made by neural network character detector in the annotations JSON format
            canny_detections : list
                Potential key bounding boxes in format [x1, y1, x2, y2] detected by canny edge detector

            Returns
            -------
            _ : dict
                New detection results after all post-processing checks in the same format as was input
        """

        # 1) find a structure in the detections -> grid == list of character rows
        rows = PostProcessor.find_rows(character_detections)
        processed = {}
        params = [rows, processed]

        # 2) check pin-pad first as alphabet layout can be recognized in pin-pad as well (next to/under numbers)
        number_checker = NumberChecker(*params)
        number_checker.check_pinpad()

        # 3) check keywords before layout as the words contain many characters which could influence layout detection
        keyword_checker = KeywordChecker(*params)
        keyword_checker.check_keywords()

        # 4) now we can finally proceed with layout detection (qwerty|alphabet|None ~ special chars / numbers)
        layout_checker = LayoutChecker(*params)
        layout = layout_checker.check_layout()
        number_checker.check_number_line(layout)  # numbers are usually above layout or among special characters

        # 5) then we check icons for special keys and their positions based on layout
        icon_checker = IconChecker(*params, layout)
        icon_checker.check_icons()

        # 6) special case - mode can sometimes mean switching to special characters and back and sometimes shift key
        keyword_checker.correct_mode_shift_key(layout)

        # 7) now remains only special characters -> select best of them
        special_chars_checker = SpecialCharactersChecker(*params, layout)
        special_chars_checker.check_special_chars()

        # 8) for qwerty layout we can try to guess space (might not have word nor icon) and shift using canny detections
        if layout == constants.qwerty and canny_detections is not None:
            canny_checker = CannyDetectionsChecker(canny_detections, processed)
            canny_checker.guess_special_keys_from_bboxes()

        # 9) lastly, some char such as xX, cC, oO etc. could have been recognized but with incorrect case -> fix that
        layout_checker.correct_capitalization()

        return processed


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-s", "--src", help="path to a detection results directory", required=True)
    parser.add_argument("-d", "--dst", help="output path for processed detection results", required=True)
    parser.add_argument("-i", "--img", help="path to image directory", required=True)
    args = parser.parse_args()
    os.makedirs(args.dst, exist_ok=True)

    for results_file in [x for x in os.listdir(args.src) if x.endswith(".json")]:

        with open(os.path.join(args.src, results_file), "r") as f:
            char_detections = json.load(f)

        # running post-processing ... this is all that is needed for the usage
        results = PostProcessor.run(char_detections)

        with open(os.path.join(args.dst, results_file), "w") as f:
            json.dump(results, f)

        filename = os.path.basename(results_file).split(".")[0]
        img = cv2.imread(os.path.join(args.img, f"{filename}.png"))

        # show original detections
        for ch, detection_values in char_detections.items():
            for det in detection_values:
                color = (0, 0, 255)
                cv2.rectangle(img, (det["bbox"][0], det["bbox"][1]), (det["bbox"][2], det["bbox"][3]), color, 1)
                cv2.putText(img, ch, (det["bbox"][0], det["bbox"][3] + 12), cv2.FONT_HERSHEY_COMPLEX, 0.5, color, 1)

        # show final processed results
        for ch, detection_values in results.items():
            for det in detection_values:
                color = (0, 255, 0) if det["conf"] != -1 else (255, 255, 0)  # green == accepted, blue == computed
                cv2.rectangle(img, (det["bbox"][0], det["bbox"][1]), (det["bbox"][2], det["bbox"][3]), color, 1)
                cv2.putText(img, ch, (det["bbox"][0], det["bbox"][1] - 3), cv2.FONT_HERSHEY_COMPLEX, 0.5, color, 1)

        cv2.imwrite(os.path.join(args.dst, f"{filename}.png"), img)
