# File: dataset_generation_base.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Base class for visualising datasets containing common attributes and methods.

import os
from os.path import join
import abc
import json
import cv2
import argparse


class DatasetVisualiserBase(metaclass=abc.ABCMeta):
    """ Provides common visualiser functionalities. """

    def __init__(self):
        self.data_path = None
        self.img_names = []
        self.bounding_boxes = {}

    @abc.abstractmethod
    def visualise_img(self, img, img_name):
        """ Renders bounding boxes on given image.

            Parameters
            ----------
            img : np.array
                Image to be displayed on which to render the bounding boxes
            img_name : str
                Name of the image which serves as a key to bounding boxes dictionary
        """

        raise NotImplementedError

    def parse_arguments(self):
        """ Parses script arguments needed for visualising. """

        parser = argparse.ArgumentParser()
        parser.add_argument("-s", "--src", help="path to dataset directory", required=True)
        parser.add_argument("-a", "--annotations", help="path to an annotations file", required=True)
        args = parser.parse_args()
        self.data_path = args.src
        self.img_names = os.listdir(self.data_path)
        with open(args.annotations) as f:
            self.bounding_boxes = json.load(f)

    def visualise(self):
        """ Performs actual visualisation of the dataset. """

        self.parse_arguments()
        for img_name in self.img_names:
            img = cv2.imread(join(self.data_path, img_name))
            self.visualise_img(img, img_name)
            cv2.imshow(img_name, img)
            cv2.waitKey(0)
            cv2.destroyAllWindows()
