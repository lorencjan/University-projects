# File: character_dataset_visualiser.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: This script is capable of visualising generated character dataset with bounding boxes.

import cv2
from dataset_visualiser_base import DatasetVisualiserBase


class CharacterDatasetVisualiser(DatasetVisualiserBase):
    """ Class capable of visualising characters with their bounding boxes. """

    def visualise_img(self, img, img_name):
        for char, bboxes in self.bounding_boxes[img_name].items():
            char = self.rename_unknown_char(char)
            for bbox in bboxes:
                x1, y1, x2, y2 = bbox
                cv2.rectangle(img, (x1, y1), (x2, y2), (0, 255, 0), 1)
                cv2.putText(img, char, (x1, y1 - 3), cv2.FONT_HERSHEY_COMPLEX, 0.5, (0, 255, 0), 1)

    @staticmethod
    def rename_unknown_char(char):
        """ Cv2 only font HERSHEY does not support some characters.
            This function renames them into text name so that they can be visualised.

            Parameters
            ----------
            char : char
                Char to potentially rename.

            Returns
            -------
            _ : char
                New or original name which is displayable.
        """

        if char == "£":
            return "libra"
        if char == "€":
            return "euro"
        if char == "÷":
            return "divide"

        return char


if __name__ == "__main__":
    visualiser = CharacterDatasetVisualiser()
    visualiser.visualise()
