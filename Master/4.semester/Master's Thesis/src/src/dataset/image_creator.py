# File: image_creator.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Class capable of creating color and gradient backgrounds.

import numpy as np


class ImageCreator:
    """ Handles color and gradiant background creation. """

    def __init__(self, width, height):
        self.width = width
        self.height = height

    def create_2d_gradient(self, start_color, end_color, horizontal=True):
        """ Creates single rgb dimension color gradient in given range.

            Parameters
            ----------
            start_color : int
                Starting gradient color value.
            end_color : int
                Ending gradient color value.
            horizontal : bool
                Direction of the gradient (horizontal vs. vertical)
            Returns
            ----------
            _ : np.array
                Gradient for single color dimension for 2D image.
        """

        linspace = np.linspace(start_color, end_color, self.width if horizontal else self.height)
        reps = (self.height if horizontal else self.width, 1)
        tile = np.tile(linspace, reps)
        return tile if horizontal else tile.T

    def create_3d_gradient(self, start_color, end_color, horizontal=(True, True, True)):
        """ Creates full color gradient in range of given colors.

            Parameters
            ----------
            start_color : (int, int, int)
                Starting gradient color.
            end_color : (int, int, int)
                Ending gradient color.
            horizontal : (bool, bool, bool)
                Direction of the gradient (horizontal vs. vertical) for each rgb color dimension.
            Returns
            ----------
            _ : np.array
                Full gradient 2D background image.
        """

        gradient = np.zeros((self.height, self.width, len(start_color)), dtype=np.float)
        for i, (s, e, h) in enumerate(zip(start_color, end_color, horizontal)):
            gradient[:, :, i] = self.create_2d_gradient(s, e, h)

        return gradient

    def create_single_color_img(self, color):
        """ Crates a single color background image.

            Parameters
            ----------
            color : (int, int, int)
                Color of the background.

            Returns
            ----------
            _ : np.array
                2D background image.
        """

        img = np.zeros((self.height, self.width, 3), np.uint8)
        img[:] = color
        return img
