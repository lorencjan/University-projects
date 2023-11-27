# File: character_detection_dataset_generator.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: This script creates a dataset for single character detection and recognition.
#              As keyboards are very contrastive (light characters on dark background or vise versa) where
#              black and white are most common, it's easier to do the detection in grayscale.
#              Therefore, grayscale light/dark characters are generated on respective backgrounds with varied contrasts.
#              Noise and moiré effects are added as the detection target is to read keyboards by camera.

from os import listdir
from os.path import join
import argparse
import cv2
import numpy as np
from numpy.random import random, randint, uniform
from PIL import Image, ImageDraw, ImageFont
from dataset_generator_base import DatasetGeneratorBase
from image_creator import ImageCreator
from noise_creator import NoiseCreator


class CharacterDatasetGenerator(DatasetGeneratorBase):

    def __init__(self, resolution=(640, 640)):
        super().__init__(resolution)
        self.fonts = {}
        self.icons = {}
        self.blur_image = False
        self.font_size_base_range = (8, 64)
        self.font_size_range = self.font_size_base_range
        self.character_set = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.,:;\'\"?!@#$£€%^&(){}[]<>/\\_+-*÷="
        self.words_set = ["SPACE", "space", "Space", "ENTER", "enter", "Enter", "SHIFT", "shift", "Shift", "TAB", "tab",
                          "Tab", "BACKSPACE", "backspace", "Backspace"]
        self.shift_chars_set = ["ABC", "abc", "123", "&123", "?123", "12#", "!#1", "+!=", "=\\<", "#+=", "!&#", "{&=",
                                "#$%", "!#$", "!@#", "#=@", "?=&", "SYM", "Sym", "1/2", "2/2"]
        self.char_dict = {c: "char" for c in self.character_set}
        for icon in ["backspace", "shift", "enter", "space", "tab"]:
            self.char_dict[icon] = "icon"

    def parse_arguments(self):
        parser = argparse.ArgumentParser()
        parser.add_argument("-d", "--dst", help="path to coco images directory", required=True)
        parser.add_argument("-f", "--fonts", help="path to fonts directory", required=True)
        parser.add_argument("-i", "--icons", help="path to key icons", required=True)
        parser.add_argument("-n", "--number", type=int, help="number of images to create", default=1)
        args = parser.parse_args()
        self.destination = args.dst
        self.num_of_images = args.number
        for font_type in [x for x in listdir(args.fonts) if x != "README.md"]:
            self.fonts[font_type] = [join(args.fonts, font_type, f) for f in listdir(join(args.fonts, font_type))]

        for icon_type in [x for x in listdir(args.icons) if x != "README.md"]:
            path = join(args.icons, icon_type)
            self.icons[icon_type] = [cv2.imread(join(path, f)) for f in listdir(path)]

    def get_background(self):
        """ Creates background image for characters. Keyboards are usually black or white.
            Hence, 1/3 chance for dark background, 1/3 light, 1/3 completely random grayscale.

            Returns
            -------
            _ : np.array
                Grayscale background image
        """

        r = random()
        if r <= 0.33:  # dark
            value = randint(0, 65)  # randint is right exclusive -> +1
        elif r <= 67:  # light
            value = randint(192, 256)
        else:          # random
            value = randint(0, 256)

        creator = ImageCreator(*self.resolution)
        return creator.create_single_color_img((value, value, value))

    def get_font(self):
        """ Randomly selects a font from available fonts.
            60 % regular vs. 40 % bold ... bold is quite common on keyboards.
            95 % normal vs. 5 % italic ... italic is not very common on keyboards.

            Returns
            -------
            _ : FreeTypeFont
                Pillow font
        """

        weight = "Regular" if random() < 0.6 else "Bold"
        style = "Normal" if random() < 0.95 else "Italic"
        font = self.get_random_from_array(self.fonts[f"{weight}_{style}"])
        min_size, max_size = self.font_size_range

        if random() < 0.5:  # prefer smaller characters
            max_size = max_size // 2

        size = randint(min_size, max_size + 1)
        return ImageFont.truetype(font, size)

    @staticmethod
    def get_available_font_colors(bg_color):
        """ Gets available grayscale color value in accordance with higher contrast to the background.
            75% - text will be at least color 92 points in contrast.
            25% - text will be at least color 24 points in contrast - not that contrastive but still readable.

            Parameters
            -------
            bg_color : int
                Background color value

            Returns
            -------
            _ : [int]
                Available font grayscale color values.
        """

        diff = 92 if random() < 0.75 else 24
        exclude = range(max(0, bg_color - diff), min(255, bg_color + diff) + 1)
        return [x for x in range(0, 255) if x not in exclude]

    def get_possible_char_coordinates(self, word_lines_prob=0.25):
        """ Creates possible coordinates for characters. Better through grid than random and check overflow.
            Keys on keyboard are in a grid anyway. Moreover, variety of fonts, sizes etc. makes it random enough.

            Parameters
            -------
            word_lines_prob : float
                Probability that a grid line doesn't have cells but is left empty for words

            Returns
            -------
            _ : ([(int, int), [int]]
                Pair of 1D array (flattened grid) with grid cell coordinates and list of lines for word generation.
        """

        # compute coordinates for grid cells
        width, height = self.resolution
        _, max_font_size = self.font_size_range
        padding = randint(max_font_size // 4, max_font_size // 2)
        x_steps = (width - 2 * padding) // max_font_size
        y_steps = (height - 2 * padding) // max_font_size
        x_coordinates = [padding + i * max_font_size for i in range(0, x_steps)]
        y_coordinates = [padding + i * max_font_size for i in range(0, y_steps)]

        # select some lines to be empty for words generation
        y_coords_len = len(y_coordinates)
        lines_for_words_idx = [randint(0, y_coords_len) for _ in range(int(y_coords_len * word_lines_prob))]
        lines = [y_coordinates[i] for i in sorted(set(lines_for_words_idx))]
        y_coordinates = sorted(set(y_coordinates) - set(lines))

        return [(x, y) for x in x_coordinates for y in y_coordinates], lines

    def put_word_on_background(self, img, word, x, y, color, filename):
        """ Prints a word on an image on a grid line specified by y. Print it character by character and move x.
            Saves printed chars and their bounding boxes.

            Parameters
            -------
            img : PIL.Image
                PIL image on which to print
            word : str
                Word or sequence of characters to print
            x : int
                X coordinate where to start printing
            y : int
                Y coordinate where to start printing - the line in grid cell for words
            color: int
                0-255 grayscale value
            filename: str
                Name of the current file so that the bounding boxes can be properly saved

            Returns
            -------
            _ : int
                X coordinate where tha last character of the word ends
        """

        font = self.get_font()
        for char in word:
            if x >= (self.resolution[0] - self.font_size_range[1]):
                return self.resolution[0]

            img.text((x, y), char, font=font, fill=(color, color, color))
            bbox = img.textbbox((x, y), char, font=font)
            self.save_bbox(bbox, char, filename)
            x = bbox[2]  # bbox is [x1, y1, x2, y2] - move to next position

        return x

    def put_chars_on_background(self, main_char, background, filename):
        """ Randomly selects coordinates, font and color and puts the characters on the background.

            Parameters
            -------
            main_char : char
                Character for which this image exists a which should be several times on it
            background : np.array
                Background image
            filename : str
                Name of the file to which the image will be saved. Here it serves as key to bounding box dictionary.

            Returns
            -------
            _ : np.array
                Background image with characters on it.
        """

        # prepare what (chars, icons) and how to render (colors, positions)
        self.bounding_boxes[filename] = {}
        max_font_size = self.font_size_range[1]
        bg_color = background[0][0][0]
        available_colors = self.get_available_font_colors(bg_color)
        available_coordinates, lines_for_words = self.get_possible_char_coordinates()
        chars, icons = self.get_chars_to_render(main_char, len(available_coordinates))

        # transform to PIL image to write with custom fonts (cv2 allows only its default)
        img = Image.fromarray(background)
        draw = ImageDraw.Draw(img)

        # render each char and save its bounding box
        for char in chars:
            font = self.get_font()
            color = self.get_random_from_array(available_colors)
            x, y = self.get_random_from_array(available_coordinates)
            available_coordinates.remove((x, y))

            draw.text((x, y), char, font=font, fill=(color, color, color))
            bbox = draw.textbbox((x, y), char, font=font)
            self.save_bbox(bbox, char, filename)

        # render words / characters close as if words ... to learn recognizing not only individual characters
        min_word_length = 4
        charset_len = len(self.character_set)
        for line_y in lines_for_words:
            last_x = randint(max_font_size // 4, max_font_size // 2)  # initial padding
            while True:
                if random() < 0.33:  # 33 % chance for special key name
                    word = self.get_random_from_array(self.words_set)
                elif random() < 0.33:  # renewed 33 % chance for special chars sequence used in shift
                    word = self.get_random_from_array(self.shift_chars_set)
                else:  # random char sequence
                    word_length = randint(min_word_length, 3 * min_word_length)
                    word = "".join([self.character_set[randint(0, charset_len)] for _ in range(word_length)])

                last_x = self.put_word_on_background(draw, word, last_x, line_y, color, filename)

                last_x += randint(max_font_size, 2 * max_font_size)  # random space between words
                if last_x >= (self.resolution[0] - max_font_size):
                    break

        # convert back to cv2 image
        img = np.array(img)

        # render each icon and save its bounding box
        for icon_name in icons:
            size = randint(16, max_font_size + 1)  # icons should be at least 16x16
            icon = self.get_random_from_array(self.icons[icon_name]).copy()
            icon = cv2.resize(icon, (size, size))
            color = self.get_random_from_array(available_colors)
            x, y = self.get_random_from_array(available_coordinates)
            available_coordinates.remove((x, y))

            icon_bg_map = icon >= (200, 200, 200)  # icon background is white, but to have some reserve (e.g. resize)
            icon_sign_map = icon < (200, 200, 200)  # icon sign is black, but basically use all that is not background
            icon = np.where(icon_bg_map, (bg_color, bg_color, bg_color), icon)
            icon = np.where(icon_sign_map, (color, color, color), icon)

            img[y:y+size, x:x+size] = icon
            bbox = self.get_icon_bbox(icon_sign_map, (x, y))
            self.save_bbox(bbox, icon_name, filename)

        return img

    def get_chars_to_render(self, main_char, coordinates_len):
        """ Gets chars/icons to render ... half is main char/icon, second half are randoms.

            Parameters
            -------
            main_char : char
                Character for which this image exists a which should be several times on it.
            coordinates_len : int
                Number of available coordinates.

            Returns
            -------
            _ : tuple
                Lists of characters and icons to render.
        """

        chars, icons = [], []
        main_chars_count = randint(coordinates_len // 4, coordinates_len // 2)  # randomly 1/4 to 1/2 of grid cells
        other_chars_count = randint(coordinates_len // 4, coordinates_len // 2)
        if self.char_dict[main_char] == "char":
            chars = list(np.repeat(main_char, main_chars_count))
        else:
            icons = list(np.repeat(main_char, main_chars_count))

        for _ in range(0, other_chars_count):
            random_char = self.get_random_from_array(list(self.char_dict.keys()))
            if self.char_dict[random_char] == "char":
                chars.append(random_char)
            else:
                icons.append(random_char)

        return chars, icons

    @staticmethod
    def get_icon_bbox(icon_sign_map, icon_coordinates):
        """ Icon images have some padding, this method extracts icon exact bounding box.

            Parameters
            -------
            icon_sign_map : np.array
                Map of the icon image with True for pixels where icon sign is and False for background.
            icon_coordinates : tuple
                Coordinates where the icon is rendered in the image.

            Returns
            -------
            _ : tuple
                Bounding box of the rendered icon.
        """

        x, y = icon_coordinates
        bbox_map = icon_sign_map[:, :, 0]
        bbox_x1, bbox_x2, bbox_y1, bbox_y2 = None, None, None, None
        for i, row in enumerate(bbox_map):
            if any(row):
                bbox_y2 = i
                if not bbox_y1:
                    bbox_y1 = i

        for i, col in enumerate(bbox_map.T):
            if any(col):
                bbox_x2 = i
                if not bbox_x1:
                    bbox_x1 = i

        return x + bbox_x1, y + bbox_y1, x + bbox_x2, y + bbox_y2

    def save_bbox(self, bbox, key, filename):
        """ Saves bounding box to local dictionary.

            Parameters
            -------
            bbox : list
                List of bounding box coordinates
            key : str
                Key to bounding box dictionary for character / icon.
            filename : str
                Name of the file to which the image will be saved. Here it serves as key to bounding box dictionary.
        """

        if key not in self.bounding_boxes[filename].keys():
            self.bounding_boxes[filename][key] = [bbox]
        else:
            self.bounding_boxes[filename][key].append(bbox)

    @staticmethod
    def convert_to_grayscale(img):
        """ Converts an image to grayscale.
            Useful as cv2.cvtColor method works weird sometimes, capable of converting white to black etc.

            Parameters
            -------
            img : np.array
                Image to convert to grayscale.

            Returns
            -------
            _ : np.array
                Original image but in grayscale.
        """

        avg_color_vals = np.average(img, axis=2)
        avg_color_vals_3d = np.expand_dims(avg_color_vals, axis=2)
        grayscale = np.concatenate((avg_color_vals_3d, avg_color_vals_3d, avg_color_vals_3d), axis=2)

        return grayscale

    def decide_if_blur_image(self, blur_prob=0.33):
        """ Decide if the image should be scaled down and up again to simulate quality reduction.

            Parameters
            -------
            blur_prob : np.array
                Probability of blurring the image.
        """

        if random() > blur_prob:
            self.font_size_range = self.font_size_base_range
            self.blur_image = False
        else:
            _, max_size = self.font_size_base_range
            self.font_size_range = (18, max_size)  # increase min font size to prevent resizing already small chars
            self.blur_image = True

    def reduce_quality_by_scaling(self, img):
        """ Conditionally scales image down and then back to simulate quality reduction.

            Parameters
            -------
            img : np.array
                Image to process.

            Returns
            -------
            _ : np.array
                Either original or blured image.
        """

        if not self.blur_image:
            return img

        scale_to = uniform(0.5, 0.8)  # how much to scale relatively to original
        width, height = self.resolution
        scale_width, scale_height = int(width * scale_to), int(height * scale_to)
        img = cv2.resize(img, (scale_width, scale_height))
        return cv2.resize(img, (width, height))  # scale back

    def generate_img(self, filename):
        self.decide_if_blur_image()
        all_chars = list(self.char_dict.keys())
        char = all_chars[self.num_of_generated % len(all_chars)]
        img = self.get_background()
        img = self.put_chars_on_background(char, img, filename)

        if random() < 0.5:  # increase probability of gaussian noise (potentially more than 1 noise)
            img = NoiseCreator.add_gaussian_noise(img)
        if random() < 0.95:
            img = NoiseCreator.add_random_noise(img)

        img = self.add_moire(img, max_lattice_order=1, special_moire_symmetry_prob=0)
        img = self.reduce_quality_by_scaling(img)

        # ensure grayscale as for example adding of noise (especially speckle) might have introduced colors
        return self.convert_to_grayscale(img)


if __name__ == "__main__":
    generator = CharacterDatasetGenerator()
    generator.generate(True)
