# File: detection_runner.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: This script runs the detections.

import os
from os.path import join
import time
import cv2
import argparse
import json
from threading import Thread

from keyboard_detector import KeyboardDetector
from character_detector import CharacterDetector
from canny_detector import CannyDetector

import sys
sys.path.insert(0, "./postprocessing")
from post_processor import PostProcessor


class DetectionRunner:
    """ Class responsible for running appropriate detections """

    def __init__(self):
        self.args = None

    def parse_arguments(self):
        """ Parses program arguments """

        parser = argparse.ArgumentParser()
        parser.add_argument("-s", "--src", help="path to a directory with images to run detection on", required=True)
        parser.add_argument("-d", "--dst", help="output directory for detection annotations", required=True)
        parser.add_argument("-v", "--verbose", action="store_true", help="prints detection time")
        parser.add_argument("-m", "--mode", choices=["keyboard", "char", "char_with_postprocessing", "full"],
                            help="One of keyboard|char|char_with_postprocessing|full specifying what detection to run")
        parser.add_argument("-cm", "--char-model", help="path to trained character detection model",
                            default="./models/model_characters_tiny.pt")
        parser.add_argument("-km", "--keyboard-model", help="path to trained keyboard detection model",
                            default="./models/model_keyboards_tiny.pt")
        parser.add_argument("--save", action="store_true", help="defines if image with detected bboxes should be saved")

        self.args = parser.parse_args()

    def run_detection(self, detect):
        """ Runs keyboard or character detection on all provided images.

            Parameters
            -------
            detect : function(img)
                Function taking an image and returning detection format results
        """

        for file in [x for x in os.listdir(self.args.src) if x.endswith(".png")]:
            img = cv2.imread(join(self.args.src, file))
            t1 = time.time()
            result = detect(img)

            if self.args.verbose:
                print(f"Detection time of {file}: {(time.time() - t1):.3f}s")

            with open(f"{join(self.args.dst, file[:-4])}.json", "w") as f:
                json.dump(result, f)

            if not self.args.save:
                continue

            for cls, bboxes in result.items():
                # cv2 cannot print these characters
                cls = "euro" if cls == "€" else "libra" if cls == "£" else "divide" if cls == "÷" else cls
                color = (255, 255, 0) if cls == "keyboard" else (0, 255, 0)  # green for chars, cyan for keyboards
                for bbox in bboxes:
                    x1, y1, x2, y2 = bbox["bbox"]
                    cv2.rectangle(img, (x1, y1), (x2, y2), color, 1)
                    cv2.putText(img, cls, (x1, y1 - 3), cv2.FONT_HERSHEY_COMPLEX, 0.5, color, 1)

            cv2.imwrite(join(self.args.dst, file), img)

    def get_char_with_postprocessing_detect_func(self):
        """ Creates a detect function for character detection with postprocessing which is expected by
            'run_detection' method. This detection has several steps, it computes parallely character
            and canny detections which are subsequently processed by the post-processing algorithm.

            Returns
            -------
            _ : function(img)
                Character detection function taking target image as a parameter and returning JSON like detections
        """

        char_detector = CharacterDetector(self.args.char_model)

        # helper local function which allows storing thread results
        def run_thread_func_with_result(func, img, thread_key, results):
            results[thread_key] = func(img)

        # detect function - it runs character and canny detections in parallel and then (post)processes their results
        def detect(img):

            results = {}
            threads = [
                Thread(target=run_thread_func_with_result, args=(char_detector.detect, img, "char", results)),
                Thread(target=run_thread_func_with_result, args=(CannyDetector.detect, img, "canny", results))
            ]

            for t in threads:
                t.start()

            for t in threads:
                t.join()

            return PostProcessor.run(results["char"], results["canny"])

        return detect

    def get_full_detection_detect_func(self):
        """ Creates a detect function for complete keyboard to post-processing detection.

            Returns
            -------
            _ : function(img)
                Detection function taking target image as a parameter and returning JSON like detections
        """

        keyboard_detector = KeyboardDetector(self.args.keyboard_model)
        char_detector = CharacterDetector(self.args.char_model)

        # helper local function which allows storing thread results
        def run_thread_func_with_result(func, img, thread_key, results):
            results[thread_key] = func(img)

        # detect function - it runs character and canny detections in parallel and then (post)processes their results
        def detect(img):
            # run keyboard detection
            keyboard_detections = keyboard_detector.detect(img)
            if len(keyboard_detections.items()) == 0:
                return {}

            # extract keyboard region for subsequent character detection
            best_keyboard_detection = sorted(keyboard_detections["keyboard"], key=lambda x: x["conf"], reverse=True)[0]
            keyboard_bbox = best_keyboard_detection["bbox"]
            keyboard = img[keyboard_bbox[1]:keyboard_bbox[3], keyboard_bbox[0]:keyboard_bbox[2]]

            # run character detection on keyboard region
            results = {}
            threads = [
                Thread(target=run_thread_func_with_result, args=(char_detector.detect, keyboard, "char", results)),
                Thread(target=run_thread_func_with_result, args=(CannyDetector.detect, keyboard, "canny", results))
            ]

            for t in threads:
                t.start()

            for t in threads:
                t.join()

            # join keyboard and character detection results
            detections = PostProcessor.run(results["char"], results["canny"])
            x_offset, y_offset = keyboard_bbox[0], keyboard_bbox[1]
            for ch, bboxes in detections.items():
                for bbox in bboxes:
                    x1, y1, x2, y2 = bbox["bbox"]
                    bbox["bbox"] = [x1 + x_offset, y1 + y_offset, x2 + x_offset, y2 + y_offset]

            detections["keyboard"] = [best_keyboard_detection]
            return detections

        return detect

    def run(self):
        """ Runs corresponding detection based on program arguments """

        self.parse_arguments()
        os.makedirs(self.args.dst, exist_ok=True)
        if self.args.mode == "keyboard":
            self.run_detection(KeyboardDetector(self.args.keyboard_model).detect)
        elif self.args.mode == "char":
            self.run_detection(CharacterDetector(self.args.char_model).detect)
        elif self.args.mode == "char_with_postprocessing":
            self.run_detection(self.get_char_with_postprocessing_detect_func())
        else:  # full detection
            self.run_detection(self.get_full_detection_detect_func())


if __name__ == "__main__":
    DetectionRunner().run()
