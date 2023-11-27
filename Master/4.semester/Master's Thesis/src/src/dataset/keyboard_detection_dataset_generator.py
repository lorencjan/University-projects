# File: keyboard_detection_dataset_generator.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: This script creates a dataset for keyboard detection and recognition. Hundreds of keyboards have been
#              downloaded from the internet which are split to train-val-test samples. They are then rendered on
#              scene images or color background. Train samples are augmented. Light, noise and moiré effects are added
#              as the detection target is to read keyboards by camera.

from os import listdir
from os.path import isfile, join
import cv2
import argparse
import numpy as np
from numpy.random import random, randint, uniform
from dataset_generator_base import DatasetGeneratorBase
from image_creator import ImageCreator
from noise_creator import NoiseCreator


class KeyboardDatasetGenerator(DatasetGeneratorBase):
    """ Class capable of generating keyboard dataset. """

    def __init__(self, resolution=(1280, 720)):
        super().__init__(resolution)
        self.keyboards = None
        self.coco = None
        self.is_train = None

    def parse_arguments(self):
        parser = argparse.ArgumentParser()
        parser.add_argument("-s", "--src", help="path to keyboard images directory", required=True)
        parser.add_argument("-c", "--coco", help="path to coco images directory", required=True)
        parser.add_argument("-d", "--dst", help="path to dataset destination directory", required=True)
        parser.add_argument("-t", "--train", type=bool, help="specify is generating training dataset", default=True)
        parser.add_argument("-n", "--number", type=int, help="number of images to create", default=25)
        args = parser.parse_args()
        self.coco = [join(args.coco, f) for f in listdir(args.coco) if isfile(join(args.coco, f))]
        self.keyboards = [join(args.src, f) for f in listdir(args.src) if isfile(join(args.src, f))]
        self.destination = args.dst
        self.num_of_images = args.number
        self.is_train = args.train

    def get_keyboard(self, resize_prob=0.25):
        """ Loads next keyboard, randomly resizes it and returns it.

            Parameters
            -------
            resize_prob : float, optional
                Probability that the keyboard is resized

            Returns
            -------
            _ : np.array
                Keyboard image
        """

        keyboard_path = self.keyboards[self.num_of_generated % len(self.keyboards)]
        keyboard = cv2.imread(keyboard_path)

        img_width, img_height = self.resolution
        kb_height, kb_width, _ = keyboard.shape

        # randomly resize keyboard if it doesn't overflow - only for training data as we want the testing preserved
        if self.is_train and random() < resize_prob and img_width - kb_width < 0 and img_height - kb_height < 0:
            resize_by = uniform(0.5, 2)
            new_width = int(min(resize_by * kb_width, img_width))
            new_height = int(min(resize_by * kb_height, img_height))
            keyboard = cv2.resize(keyboard, (new_width, new_height), interpolation=cv2.INTER_CUBIC)
            kb_height, kb_width, _ = keyboard.shape

        # check for keyboard overflow
        if img_width - kb_width < 0:  # keep full width if overflows
            keyboard = cv2.resize(keyboard, (img_width, int(kb_height * img_width / kb_width)))
            kb_height, kb_width, _ = keyboard.shape
        if img_height - kb_height < 0:  # no keyboard is full height, max half the screen -> reduce to half
            keyboard = cv2.resize(keyboard, (int(kb_width * (img_height // 2) / kb_height), img_height // 2))

        return keyboard

    def get_background(self, keyboard):
        """ Gets background for the keyboard. There is a 50 % chance for scene background selected from coco dataset.
            Another 40 % is color of keyboard borders so that the keyboard blends in. Remaining 10 % is for completely
            random color or gradient as a background.

            Parameters
            -------
            keyboard : np.array
                Keyboard image to get the border color

            Returns
            -------
            _ : np.array
                Background image
        """

        r = random()
        if r <= 0.5:  # coco
            img_path = self.coco[randint(0, len(self.coco) - 1)]
            img = cv2.imread(img_path)
            return cv2.resize(img, self.resolution, interpolation=cv2.INTER_CUBIC)

        width, height = self.resolution
        creator = ImageCreator(width, height)
        if r <= 0.9:  # background of keyboard edges
            top, bottom, left, right = keyboard[0], keyboard[-1], keyboard[1:-1, 0], keyboard[1:-1, -1]
            border_pixels = np.concatenate([top, bottom, left, right])
            color = [int(border_pixels[:, i].mean()) for i in range(0, border_pixels.shape[1])]
            return creator.create_single_color_img(color)

        # else random color/gradient
        color = (randint(0, 255), randint(0, 255), randint(0, 255))
        if random() < 0.25:
            return creator.create_single_color_img(color)

        gradient_target = (randint(0, 255), randint(0, 255), randint(0, 255))
        gradient_direction = (random() < 0.5, random() < 0.5, random() < 0.5)
        return creator.create_3d_gradient(color, gradient_target, gradient_direction)

    def put_keyboard_on_background(self, keyboard, background, filename, transparency_prob=0.33):
        """ Randomly selects keyboard coordinates on the background so that it doesn't overflow.
            Then it puts the keyboard there with random transparency and saves the keyboard bounding box.

            Parameters
            -------
            keyboard : np.array
                Keyboard image to put on the background image
            background : np.array
                Background image
            filename : string
                Name of the file it is going to be saved in which serves as a key to bounding box dictionary
            transparency_prob : float, optional
                Probability with which keyboard transparency change will be applied

            Returns
            -------
            _ : np.array
                Background image with keyboard on it
        """

        background = background.astype(np.uint8)
        b_height, b_width, _ = background.shape
        k_height, k_width, _ = keyboard.shape

        # find a random place in bounds
        x1 = randint(0, b_width - k_width) if b_width - k_width > 0 else 0
        y1 = randint(0, b_height - k_height) if b_height - k_height > 0 else 0
        x2 = x1 + k_width
        y2 = y1 + k_height

        # put keyboard to the image
        self.bounding_boxes[filename] = [x1, y1, x2, y2]
        overlay = np.zeros_like(background, np.uint8)
        overlay[y1:y2, x1:x2] = keyboard
        mask = overlay.astype(bool)

        # conditionally add transparency
        alpha = 1 if random() > transparency_prob else uniform(0.5, 1)
        background[mask] = cv2.addWeighted(background, 1 - alpha, overlay, alpha, 0)[mask]
        return background

    @staticmethod
    def change_brightness(img):
        """ Randomly selects gamma value using which it changes the brightness.

            Parameters
            -------
            img : np.array
                Image to which change the brightness

            Returns
            -------
            _ : np.array
                Original image with different brightness
        """

        gamma = uniform(0.5, 1.5)
        inv_gamma = 1. / gamma
        table = np.array([((i / 255.0) ** inv_gamma) * 255 for i in np.arange(0, 256)])
        return cv2.LUT(img.astype("uint8"), table.astype("uint8"))

    def generate_img(self, filename):
        keyboard = self.get_keyboard()
        img = self.get_background(keyboard)
        img = self.put_keyboard_on_background(keyboard, img, filename)
        if random() < 0.5:
            img = self.change_brightness(img)
        if random() < 0.9:
            img = NoiseCreator.add_random_noise(img)

        img = self.add_moire(img)
        return img


if __name__ == "__main__":
    generator = KeyboardDatasetGenerator()
    generator.generate(True)
