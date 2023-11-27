# File: keyboard_dataset_visualiser.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: This script is capable of visualising generated keyboard dataset with bounding boxes.

import cv2
from dataset_visualiser_base import DatasetVisualiserBase


class KeyboardDatasetVisualiser(DatasetVisualiserBase):
    """ Class capable of visualising keyboards with their bounding boxes. """

    def visualise_img(self, img, img_name):
        x1, y1, x2, y2 = self.bounding_boxes[img_name]
        cv2.rectangle(img, (x1, y1), (x2, y2), (0, 255, 0), 1)


if __name__ == "__main__":
    visualiser = KeyboardDatasetVisualiser()
    visualiser.visualise()
