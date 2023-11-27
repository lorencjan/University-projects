# File: dataset_generation_base.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Base class for generating datasets containing common attributes and methods.

import os
from os.path import join
import json
import abc
import cv2
import time
import numpy as np
from numpy.random import randint
from noise_creator import NoiseCreator
from moire_creator import MoireCreator


class DatasetGeneratorBase(metaclass=abc.ABCMeta):
    """ Provides common generator functionalities. """

    def __init__(self, resolution):
        self.resolution = resolution
        self.bounding_boxes = {}
        self.destination = None
        self.num_of_images = 0
        self.num_of_generated = 0

    @abc.abstractmethod
    def parse_arguments(self):
        """ Parses script arguments needed for dataset generation. """

        raise NotImplementedError

    @abc.abstractmethod
    def generate_img(self, filename):
        """ Generates single image in dataset generation.

            Parameters
            ----------
            filename : string
                Name of the file to which store the image
        """

        raise NotImplementedError

    @staticmethod
    def create_dir(directory):
        """ Helper function for directory creation.

            Parameters
            ----------
            directory : string
                Directory path to create
        """

        if not os.path.exists(directory):
            os.makedirs(directory)

    @staticmethod
    def get_random_from_array(arr):
        """ Helper function for selecting random item from a list.

            Parameters
            ----------
            arr : []
                Array from which to select
            Returns
            ----------
            _ : any
                Random item from given list
        """

        i = randint(0, len(arr))
        return arr[i]

    def add_moire(self, img, max_lattices=2, max_lattice_order=3, special_moire_symmetry_prob=0.5):
        """ Adds moiré effect to the image.

            Parameters
            ----------
            img : np.array
                Original image
            max_lattices : int, optional
                Maximum number of lattices to be the resulting lattice combined of.
            max_lattice_order : int, optional
                Defines how much the lattice is resolved as single dots.
            special_moire_symmetry_prob : float, optional
                Most common moiré effect are lines (symmetry=1).
                This parameter specifies probability of more intricate patterns.

            Returns
            ----------
            _ : np.array
                Image with moiré effect on it
        """

        width, height = self.resolution
        num_of_lattices = randint(1, max_lattices + 1)
        creator = MoireCreator(width)
        lattice = creator.combine_lattice(num_of_lattices, max_lattice_order, special_moire_symmetry_prob)[:height, :]
        lattice_3d = np.dstack([lattice for _ in range(0, 3)])
        return img + lattice_3d if img.mean() < 128 else img - lattice_3d

    def save_annotations(self, filename="annotations.json"):
        """ Helper method that saves bounding boxes to a json.

            Parameters
            ----------
            filename : string, optional
                Name of the file for image bounding boxes annotations.
        """

        with open(join(self.destination, filename), "w") as f:
            json.dump(self.bounding_boxes, f)

    def generate(self, timed=False):
        """ Performs actual generation of the dataset.

            Parameters
            ----------
            timed : bool, optional
                If true, duration of the generation is printed to standard output.
        """

        self.parse_arguments()
        self.create_dir(self.destination)
        start = time.time()
        for i in range(0, self.num_of_images):
            filename = f"{self.num_of_generated + 1}.png"
            img = self.generate_img(filename)
            cv2.imwrite(join(self.destination, filename), img)
            self.num_of_generated = self.num_of_generated + 1
            print(f"Generated: {self.num_of_generated}/{self.num_of_images}", end="\r")

        self.save_annotations()
        if timed:
            print(f"Elapsed: {time.time() - start}")
