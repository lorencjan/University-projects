# File: evaluate.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Computes detection metrics (precision, recall, F1 score) for postprocessing results.

import os
import argparse
import json
import cv2

import layouts
import constants
from key_checker_base import KeyCheckerBase


colors = {
    "annotation_positive": (0, 255, 255),        # yellow for detected annotation
    "annotation_negative": (255, 0, 255),        # magenta for false negative
    "annotation_other_positive": (255, 255, 0),  # cyan if not detected but detected other (e.g. multiple shifts)
    "detection_positive": (0, 255, 0),           # green for true positive
    "detection_negative": (0, 0, 255)            # red for false positive
}


class Stats:
    """ Class responsible for computing precision, recall and F1 statistics. """

    def __init__(self, cls):
        """ Takes detection class name and initializes counters. """

        self.cls = cls
        self.true_positives = 0   # detection result matches the source of truth
        self.false_positives = 0  # detection result doesn't match the source of truth
        self.false_negatives = 0  # there isn't any matching detection for a source of truth item

    def count(self):
        """ Computes total number of occurrences. """

        return self.true_positives + self.false_positives

    def recall(self):
        """ Computes recall metric. """

        return (self.true_positives / (self.true_positives + self.false_negatives)
                if (self.true_positives + self.false_negatives) > 0 else 0)

    def precision(self):
        """ Computes precision metric. """

        return self.true_positives / (self.true_positives + self.false_positives) if self.count() > 0 else 0

    def f1(self):
        """ Computes F1 score. """

        if self.count() == 0:
            return 0

        precision = self.precision()
        recall = self.recall()
        return precision * recall / ((precision + recall) / 2) if precision + recall > 0 else 0

    def print(self):
        """ Creates a formatted output class row. """

        return f"{self.cls: >15}{'': >5}"\
               f"{self.count(): >10}{'': >5}" \
               f"{f'{self.precision():.3f}': >10}{'': >5}" \
               f"{f'{self.recall():.3f}': >10}{'': >5}"\
               f"{f'{self.f1():.3f}': >10}\n"


class ImageData:
    """ Class storing image related date - image itself, detected characters and expected character annotations. """

    def __init__(self, img, detections, annotations):
        self.img = img
        self.detections = detections
        self.annotations = annotations


class Evaluator:
    """ Class responsible for running the detection results evaluation """

    def __init__(self):
        self.args = None
        char_classes = layouts.alphabet + str.upper(layouts.alphabet) + layouts.numbers + layouts.special_chars
        self.classes = [*char_classes] + constants.keywords + [constants.mode, constants.page]

    def parse_args(self):
        """ Parses program arguments. """

        parser = argparse.ArgumentParser()
        parser.add_argument("-a", "--annotations", help="path to annotation file (source of truth)", required=True)
        parser.add_argument("-s", "--src", help="path to a directory with detection results to evaluate", required=True)
        parser.add_argument("-i", "--img", help="path to a directory with original images", required=True)
        parser.add_argument("-d", "--dst", help="path to a directory where to save results", required=True)
        self.args = parser.parse_args()
        os.makedirs(self.args.dst, exist_ok=True)

    def load_data(self):
        """ Loads images, detection and expected annotations.

            Returns
            -------
            _ : dict
                Object with ImageData for each file
        """

        with open(self.args.annotations, "r") as f:
            annotations = json.load(f)

        data = {}
        for results_file in [x for x in os.listdir(self.args.src) if x.endswith(".json")]:
            with open(os.path.join(self.args.src, results_file)) as f:
                detections = json.load(f)

            filename = os.path.basename(results_file).split(".")[0]
            img = cv2.imread(os.path.join(self.args.img, f"{filename}.png"))
            data[filename] = ImageData(img, detections, annotations[f"{filename}.png"])

        return data

    def compute_stats(self, data):
        """ Loads images, detection and expected annotations.

            Parameters
            -------
            data : dict
                Object with ImageData for each file

            Returns
            -------
            _ : dict
                Object with filled instances of Stats class for each file
        """

        statistics = {ch: Stats(ch) for ch in self.classes}
        for filename, img_data in data.items():
            for ch, annotations in img_data.annotations.items():
                # not detected what should have -> all are false negatives
                if ch not in img_data.detections.keys():
                    statistics[ch].false_negatives += len(annotations)
                    for a in annotations:
                        self.render_bbox(img_data.img, ch, a, colors["annotation_negative"])
                    continue

                # check positives against expected
                found_any = False
                for detection in img_data.detections[ch]:
                    bbox = detection["bbox"]
                    found = next(iter(a for a in annotations if KeyCheckerBase.rectangles_overlap(bbox, a)), None)
                    if found:
                        statistics[ch].true_positives += 1
                        annotations.remove(found)
                        found_any = True
                        self.render_bbox(img_data.img, None, found, colors["annotation_positive"])
                        self.render_bbox(img_data.img, ch, bbox, colors["detection_positive"])
                    else:
                        statistics[ch].false_positives += 1
                        self.render_bbox(img_data.img, ch, bbox, colors["detection_negative"])

                del img_data.detections[ch]

                # some keys such as shift can be expected multiple times but the detection selects one which is enough
                # -> the rest is not important -> detection is correct
                if not found_any:
                    statistics[ch].false_negatives += len(annotations)

                # render unprocessed expected bboxes
                for a in annotations:
                    color = colors["annotation_other_positive" if found_any else "annotation_negative"]
                    self.render_bbox(img_data.img, ch, a, color)

            for ch, detections in img_data.detections.items():
                for det in detections:
                    statistics[ch].false_positives += 1
                    self.render_bbox(img_data.img, ch, det["bbox"], colors["detection_negative"])

            cv2.imwrite(os.path.join(self.args.dst, f"{filename}.png"), img_data.img)

        return statistics

    def print_stats(self, statistics):
        """ Creates formatted text output for given statistics.

            Parameters
            -------
            statistics : dict
                Object with filled instances of Stats class for each file

            Returns
            -------
            _ : str
                Formatted statistics text result
        """

        result = f"{'Class:': >15}{'': >5}{'Count:': >10}{'': >5}{'Precision:': >10}{'': >5}" \
                 f"{'Recall:': >10}{'': >5}{'F1:': >10}\n"
        result += self.print_group_stats("all", self.classes, statistics) + "\n"

        result += " Groups ".center(75, "*") + "\n"
        groups = [
            ("alphabet", layouts.alphabet + str.upper(layouts.alphabet)),
            ("numbers", layouts.numbers[1:]),
            ("alphanumeric", layouts.alphabet + str.upper(layouts.alphabet) + layouts.numbers[1:]),
            ("special keys", constants.keywords + [constants.mode, constants.page]),
            ("special chars", layouts.special_chars)
        ]
        for name, classes in groups:
            result += self.print_group_stats(name, classes, statistics)

        result += "\n" + " Individual keys ".center(75, "*") + "\n"
        for ch, stats in statistics.items():
            result += stats.print()

        with open(os.path.join(self.args.dst, "_statistics.txt"), "w") as f:
            f.write(result)

        return result

    @staticmethod
    def print_group_stats(name, classes, statistics):
        """ Creates text statistics for a subset of detection classes.

            Parameters
            -------
            name : str
                Name of the group == subset of detection classes
            classes : list
                Subset of detection classes (list of names)
            statistics : dict
                Object with filled instances of Stats class for each file

            Returns
            -------
            _ : str
                Formatted output group row
        """

        stats = Stats(name)
        for c in classes:
            stats.true_positives += statistics[c].true_positives
            stats.false_positives += statistics[c].false_positives
            stats.false_negatives += statistics[c].false_negatives

        return stats.print()

    @staticmethod
    def render_bbox(img, ch, bbox, color):
        """ Renders bounding box on given image.

            Parameters
            -------
            img : np.array
                Cv2 loaded image
            ch : str
                Name of the bounding box
            bbox : list
                Bounding box coordinates in format [x1, y1, x2, y2]
            color : tuple
                Cv2 color tuple
        """

        cv2.rectangle(img, (bbox[0], bbox[1]), (bbox[2], bbox[3]), color, 1)
        if ch is not None:
            ch = "euro" if ch == "€" else "libra" if ch == "£" else "divide" if ch == "÷" else ch
            cv2.putText(img, ch, (bbox[0], bbox[1] - 3), cv2.FONT_HERSHEY_COMPLEX, 0.5, color, 1)


if __name__ == "__main__":
    evaluator = Evaluator()
    evaluator.parse_args()
    data = evaluator.load_data()
    stats = evaluator.compute_stats(data)
    text = evaluator.print_stats(stats)
    print(text)
