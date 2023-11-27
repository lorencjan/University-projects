# File: dataset_splitter.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Script that splits target images to train, validation and test samples.

import os
import cv2
from sklearn.model_selection import train_test_split
import argparse


class DatasetSplitter:
    """ Class capable of splitting images to train, validation and test samples. """

    def __init__(self):
        self.train = {}
        self.val = {}
        self.test = {}
        self.src_paths = {}
        self.dst_path = None
        self.processed_images = 0
        self.total_images = 0

    def parse_arguments(self):
        """ Parses script arguments needed for splitting. """

        parser = argparse.ArgumentParser()
        parser.add_argument("-s", "--src", help="path to images directory", required=True)
        parser.add_argument("-d", "--dst", help="path where to save split images", required=True)
        args = parser.parse_args()
        data_path = args.src
        self.dst_path = os.path.join(os.getcwd(), args.dst)
        for directory, subdirectories, files in os.walk(data_path):
            for category in subdirectories:
                self.src_paths[category] = {}

            current_category = directory.split(os.sep)[-1]
            if current_category in self.src_paths.keys():
                self.src_paths[current_category] = [os.path.join(directory, x) for x in files]

        self.total_images = sum(len(x) for x in self.src_paths.values())

    def train_val_test_split(self, train=0.7, val=0.2, test=0.1):
        """ Does actual splitting. By default, it splits to standard 70:20:10.

            Parameters
            ----------
            train : int, optional
                Train samples split size
            val : int, optional
                Validation samples split size
            test : int, optional
                Test samples split size
        """

        for category, images in self.src_paths.items():
            cat_train, cat_test = train_test_split(images, test_size=1-train, shuffle=True)
            cat_val, cat_test = train_test_split(cat_test, test_size=test/(val+test), shuffle=True)
            self.train[category] = cat_train
            self.val[category] = cat_val
            self.test[category] = cat_test

    def save_dataset(self, img_paths, dst_path):
        """ Does actual splitting. By default, it splits to standard 70:20:10.

            Parameters
            ----------
            img_paths : Array
                Paths to images (already split) to save to specific destination
            dst_path : str
                Destination path where to save the images
        """

        if not os.path.exists(dst_path):
            os.makedirs(dst_path)

        for category in img_paths:
            for file_path in img_paths[category]:
                filename = file_path.split(os.sep)[-1]
                cv2.imwrite(f"{dst_path}{os.sep}{filename}", cv2.imread(file_path))
                self.processed_images += 1
                print(f"Processed: {self.processed_images}/{self.total_images}", end='\r')

    def split(self):
        """ Performs all splitting operation. """

        self.parse_arguments()
        self.train_val_test_split()
        self.save_dataset(self.train, f"{self.dst_path}{os.sep}train")
        self.save_dataset(self.val, f"{self.dst_path}{os.sep}val")
        self.save_dataset(self.test, f"{self.dst_path}{os.sep}test")


if __name__ == "__main__":
    splitter = DatasetSplitter()
    splitter.split()
