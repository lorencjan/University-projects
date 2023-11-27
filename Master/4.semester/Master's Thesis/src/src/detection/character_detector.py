# File: character_detector.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: This file provides a character detector.

import sys
sys.path.insert(0, "../training")

import cv2
import numpy as np
import torch
import torchvision

from yolov7_annotation_convert_maps import label_to_char_map
from detector_base import DetectorBase


class CharacterDetector(DetectorBase):
    """ Class responsible for character detection using a trained model """

    def __init__(self, model_path):
        super().__init__(model_path)
        self.max_font_size = 64         # max font size to compute split image overlap

    def split_img(self, img, offset=0, max_depth=2, limit=640, min_ratio=0):
        """ Keyboards are very often very wide which results in huge scale down to 640x640 with respect to height.
            To preserve image quality for detection, the image is split to smaller more squarish images. The detection
            is then run on these (several detections)

           Parameters
           ----------
           img : np.array
               Image to be split
           offset : int
               Defines X axis offset of currently splitting image
               e.g. splitting of 1000px image results in 2  500px images, first with offset 0 and second with offset 500
           max_depth : int
               Recursion depth defining how many splits there can be. Default 2 means it splits to max 4 images.
           limit : int
               Basically width of detection window, no need to split image if it's smaller
           min_ratio : float, optional
               If height to width image ratio is smaller than this parameter, split anyway to enforce more
               squarish images which can be scaled up to detection window

           Returns
           -------
           _ : list(tuple)
               List of split images with their offsets
       """

        height, width, _ = img.shape
        limit = max(limit, height)
        if (width < limit and height / width > min_ratio) or max_depth == 0:
            return [(img, offset)]

        img_half = width // 2
        overlap = self.max_font_size // 2
        right_offset = img_half - overlap + (0 if width % 2 == 0 else 1)  # off by 1 if odd width (we want same sizes)
        left_half_images = self.split_img(img[:, :img_half + overlap], offset, max_depth-1)
        right_half_images = self.split_img(img[:, right_offset:], offset + right_offset, max_depth-1)
        return left_half_images + right_half_images

    def detect(self, img):
        """ Runs character detection on an image. Firstly, it converts it to grayscale and applies noise reduction.
            Then the image is split to mode squarish ones to preserve quality. Model detection is run on each partial
            image in a batch. Results are merged and returned in a dictionary.

           Parameters
           ----------
           img : np.array
               Image to run detection on

           Returns
           -------
           _ : dict
               List of bboxes with their confidence score for each character
       """

        img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)    # convert to grayscale
        img = cv2.bilateralFilter(img, 15, 30, 70)     # reduce noise while preserving edges
        img = np.expand_dims(img, axis=2)              # model still requires 3 channel image
        img = np.concatenate((img, img, img), axis=2)  # expand single dimension gray to 3 channel gray

        # run detections on partial images
        partial_images = self.split_img(img)
        detections = self.model_detect([image for image, _ in partial_images])
        if len(detections) == 0:
            return {}

        for i, detection in enumerate(detections):
            x_offset = partial_images[i][1]
            detection += torch.tensor([x_offset, 0, x_offset, 0, 0, 0]).to(self.device)

        # merge results and run non-maximum-suppression to remove same detections from overlapping regions
        results = torch.cat(detections)
        boxes, scores = results[:, :4], results[:, 4]
        indices = torchvision.ops.nms(boxes, scores, 0.5)

        return self.convert_detections_to_json(results[indices], label_to_char_map)
