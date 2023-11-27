# File: coco_selector.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: This script filters coco dataset. For keyboard dataset generation keyboards are rendered on random
#              scene images. Those come from coco dataset. As we aim for hd images, we want to use only landscape
#              images with similar ratio so that we minimize distortion.

import os
from os.path import join
import cv2
import argparse


class CocoSelector:
    """ Class responsible for selecting only images in similar ratio to hd. """

    def __init__(self):
        self.removed = 0
        self.total_images = 0
        self.img_paths = []

    def parse_arguments(self):
        """ Parses script arguments needed for dataset generation. """

        parser = argparse.ArgumentParser()
        parser.add_argument("-s", "--src", help="path to coco images directory", required=True)
        args = parser.parse_args()
        self.img_paths = [join(args.src, x) for x in os.listdir(args.src)]
        self.img_paths.remove(join(args.src, "README.md"))
        self.total_images = len(self.img_paths)

    def process_img(self, img_path):
        """ Finds out if the image is in similar ratio as hd a deletes it if not.

            Parameters
            ----------
            img_path : string
                Path to a coco image
        """

        img = cv2.imread(img_path)
        height, width = img.shape[0], img.shape[1]
        ratio = height / width           # we want only landscape images as we aim for hd (720/1280 = 0.5625)
        if ratio < 0.5 or ratio > 0.67:  # not too flat nor too squarish -> keep only those between 1:2 and 2:3
            os.remove(img_path)
            self.removed += 1

    def run_selection(self):
        """ Runs the actual selection. """

        self.parse_arguments()
        for i, path in enumerate(self.img_paths):
            self.process_img(path)
            print(f"Processed: {i + 1}/{self.total_images}", end='\r')

        print(f"Removed {self.removed} files out of {self.total_images}")


if __name__ == "__main__":
    selector = CocoSelector()
    selector.run_selection()
