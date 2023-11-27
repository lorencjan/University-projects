# File: dataset_splitter.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: For testing of detection postprocessing, data need to be annotated.
#              This was done manually using CVAT app. Annotations were downloaded
#              in "COCO 1.0" format and need to be transformed to the format used
#              in my generated datasets for both uniformity and simplicity.

import os
import json
import argparse


class AnnotationProcessor:
    """ Class transforming COCO 1.0 annotations to simpler annotation format. """

    def __init__(self):
        self.src = None
        self.dst = None

    def parse_arguments(self):
        """ Parses script arguments. """

        parser = argparse.ArgumentParser()
        parser.add_argument("-s", "--src", help="path to annotations file to be processed", required=True)
        parser.add_argument("-d", "--dst", help="path to directory where to save the annotations", required=True)
        args = parser.parse_args()
        self.src = args.src
        self.dst = args.dst

    @staticmethod
    def transform_bbox(coco1_bbox):
        """ Bboxes in COCO 1.0 are in [top-left-x, top-left-y, width, height] format,
            whereas I use x2, y2 instead of width and height.

            Parameters
            -------
            coco1_bbox : list
                COCO 1.0 bbox in [top-left-x, top-left-y, width, height] format.

            Returns
            -------
            _ : list
                Bbox in [x1, y1, x2, y2] format.
        """

        x1, y1, width, height = coco1_bbox
        return [round(x1), round(y1), round(x1 + width), round(y1 + height)]

    @staticmethod
    def transform_category(category):
        """ Characters €, £, ÷ got encoded weirdly from CVAT. This methd fixes that

            Parameters
            -------
            category : char
                Label category == character which to fix.

            Returns
            -------
            _ : char
                Original character or fixed €, £, ÷.

        """

        if category == '\u00e2\u201a\u00ac':
            return '€'
        if category == '\u00c2\u00a3':
            return '£'
        if category == '\u00c3\u00b7':
            return '÷'

        return category

    def load_annotations(self):
        """ Loads annotations file to process and extracts relevant information.

            Returns
            -------
            _ : tuple
                Map dictionary (id:value) for categories (characters), images and list of annotations with bboxes.
        """

        with open(self.src) as f:
            data = json.load(f)

        categories = {x["id"]: self.transform_category(x["name"]) for x in data["categories"]}
        images = {x["id"]: x["file_name"] for x in data["images"]}
        annotations = [{"img": x["image_id"], "category": x["category_id"], "bbox": self.transform_bbox(x["bbox"])}
                       for x in data["annotations"]]

        return categories, images, annotations

    def save_annotations(self, annotations):
        """ Saves the annotations to destination given in script arguments. """

        if not os.path.exists(self.dst):
            os.makedirs(self.dst)

        with open(os.path.join(self.dst, "annotations.json"), "w") as f:
            json.dump(annotations, f)

    def run(self):
        """ Runs the executions. Loads the annotations, maps ids to names and saves it. """

        self.parse_arguments()
        categories, images, annotations = self.load_annotations()
        result = {img_name: {} for img_name in images.values()}
        for annotation in annotations:
            img = images[annotation["img"]]
            category = categories[annotation["category"]]
            bbox = annotation["bbox"]
            if result[img].get(category) is None:
                result[img][category] = [bbox]
            else:
                result[img][category].append(bbox)

        self.save_annotations(result)


if __name__ == "__main__":
    processor = AnnotationProcessor()
    processor.run()
