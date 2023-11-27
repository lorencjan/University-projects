# File: yolov7_annotation_converter.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: While my datasets' annotations are single jsons for whole image group
#              (1 json for all training set etc.), YOLOv7 has .txt a file for each image.
#              Moreover, my format is [x1, y1, x2, y3] coordinates, whereas YOLOv7 uses
#              [x_bbox_center, y_bbox_center, width, height] normalized between 0-1.
#              It also uses numerical labels for bbox classes starting from 0 so
#              instead of JSON's {"character": [x1, y1, x2, y3]} I need
#              a .txt with lines: label x_bbox_center y_bbox_center width height.

import os
import argparse
import json
from yolov7_annotation_convert_maps import *


class YOLOv7AnnotationConverter:
    """ Converts my bboxes to YOLOv7 format. """

    def __init__(self):
        self.img_width = None
        self.img_height = None
        self.annotations_path = None
        self.dst_path = None
        self.from_yolov7 = False

    def parse_arguments(self):
        """ Parses script arguments needed for conversion. """

        parser = argparse.ArgumentParser()
        parser.add_argument("-a", "--annotations", help="path to an annotations file", required=True)
        parser.add_argument("-iw", "--width", type=int, help="width of target images", required=True)
        parser.add_argument("-ih", "--height", type=int, help="height of target images", required=True)
        parser.add_argument("-d", "--dst", help="destination where to store the yolov7 annotations", required=True)
        parser.add_argument("-i", "--inverse", help="if set, conversion is done from yolov7", action="store_true")
        args = parser.parse_args()
        self.img_width = args.width
        self.img_height = args.height
        self.annotations_path = args.annotations
        self.dst_path = args.dst
        self.from_yolov7 = args.inverse

    def convert_bbox_to_yolov7(self, bbox):
        """ Converts my dataset's bbox to yolov7 format.

            Parameters
            ----------
            bbox : list
                List of single bbox coordinates [x1, y1, x2, y2]

            Returns
            -------
            _ : list
                List of single yolov7 bbox coordinates [x, y, width, height]
        """

        x1, y1, x2, y2 = bbox
        x = (x1 + x2) / 2 / self.img_width
        y = (y1 + y2) / 2 / self.img_height
        width = (x2 - x1) / self.img_width
        height = (y2 - y1) / self.img_height

        return x, y, width, height

    def convert_bbox_from_yolov7(self, bbox):
        """ Converts yolov7 bbox to my dataset's format.

            Parameters
            ----------
            bbox : list
                List of single yolov7 bbox coordinates [x, y, width, height]

            Returns
            -------
            _ : list
                List of single bbox coordinates [x1, y1, x2, y2]
        """

        x, y, width, height = bbox
        half_width = width / 2
        half_height = height / 2
        x1 = (x - half_width) * self.img_width
        x2 = (x + half_width) * self.img_width
        y1 = (y - half_height) * self.img_height
        y2 = (y + half_height) * self.img_height

        return round(x1), round(y1), round(x2), round(y2)

    def parse_keyboard_to_yolov7(self, value):
        """ Converts my dataset's keyboard value to yolov7 format.

            Parameters
            ----------
            value : list
                Bounding box of the keyboard - json annotation dict value

            Returns
            -------
            _ : str
                String with .txt yolov7 format value
        """

        x, y, width, height = self.convert_bbox_to_yolov7(value)
        return f"{keyboard_to_label_map['keyboard']} {x} {y} {width} {height}"

    def parse_keyboard_from_yolov7(self, value):
        """ Converts keyboards yolov7 value to my dataset's format.

            Parameters
            ----------
            value : str
                String with label and bbox - line in .txt annotation file

            Returns
            -------
            _ : list
                List of single bbox coordinates [x1, y1, x2, y2] - value for json annotation file key
        """

        bbox = [float(x) for x in value.split(" ")[1:]]
        return self.convert_bbox_from_yolov7(bbox)

    def parse_chars_to_yolov7(self, value):
        """ Converts bboxes of all characters in my dataset's annotation file to yolov7 format.

            Parameters
            ----------
            value : dict
                Dictionary of char:[bbox] - json annotation dict value

            Returns
            -------
            _ : str
                String with .txt yolov7 format values (lines) for each character's bounding box
        """

        lines = []
        for char, bboxes in value.items():
            label = char_to_label_map[char]
            for bbox in bboxes:
                x, y, width, height = self.convert_bbox_to_yolov7(bbox)
                lines.append(f"{label} {x} {y} {width} {height}")

        return "\n".join(lines)

    def parse_chars_from_yolov7(self, value):
        """ Converts all characters' bboxes in yolov7 .txt file to my dataset's json value.

            Parameters
            ----------
            value : str
                String with several lines of label and bbox - line in .txt annotation file

            Returns
            -------
            _ : dict
                Dictionary with keys as characters and values as list of [x1, y1, x2, y2] bboxes
        """

        lines = value.split("\n")
        result = {}
        for line in lines:
            vals = line.split(" ")
            label = label_to_char_map[int(vals[0])]
            yolov7_bbox = [float(x) for x in vals[1:]]
            bbox = self.convert_bbox_from_yolov7(yolov7_bbox)
            if label in result.keys():
                result[label].append(bbox)
            else:
                result[label] = [bbox]

        return result

    def parse_annotations_to_yolov7(self):
        """ Goes through annotations, converts them and saves them. """

        with open(self.annotations_path, "r") as f:
            bounding_boxes = json.load(f)

        os.makedirs(self.dst_path, exist_ok=True)
        for filename, value in bounding_boxes.items():
            content = self.parse_keyboard_to_yolov7(value) if type(value) == list else self.parse_chars_to_yolov7(value)
            dst_path = os.path.join(self.dst_path, filename.replace(".png", ".txt"))
            with open(dst_path, "w") as f:
                f.write(content)

    def parse_annotations_from_yolov7(self):
        """ Goes through annotations, converts them and saves them. """

        annotation_dict = {}
        for filename in os.listdir(self.annotations_path):
            with open(os.path.join(self.annotations_path, filename), "r") as f:
                content = f.read()

            key = filename.replace(".txt", ".png")
            annotation_dict[key] = (self.parse_chars_from_yolov7(content) if "\n" in content
                                    else self.parse_keyboard_from_yolov7(content))

        os.makedirs(os.path.dirname(self.dst_path), exist_ok=True)
        with open(self.dst_path, "w") as f:
            json.dump(annotation_dict, f)


if __name__ == "__main__":
    converter = YOLOv7AnnotationConverter()
    converter.parse_arguments()
    if converter.from_yolov7:
        converter.parse_annotations_from_yolov7()
    else:
        converter.parse_annotations_to_yolov7()
