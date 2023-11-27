# File: canny_detector.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: This file provides a canny edge detector class.

import os
import cv2
import numpy as np
import argparse
from statistics import median


class CannyDetector:
    """ Namespace class responsible for basic canny edge detection"""

    @staticmethod
    def apply_clahe(img):
        """ Enhances contrast of an image using clahe method.
            Code borrowed from:
            https://stackoverflow.com/questions/39308030/how-do-i-increase-the-contrast-of-an-image-in-python-opencv

            Parameters
            -------
            img : np.array
                Cv2 loaded image

            Returns
            -------
            _ : np.array
                Input image with enhanced contrast
        """

        # converting to LAB color space
        lab = cv2.cvtColor(img, cv2.COLOR_BGR2LAB)
        l_channel, a, b = cv2.split(lab)

        # Applying CLAHE to L-channel
        clahe = cv2.createCLAHE(clipLimit=1.0, tileGridSize=(6, 6))
        cl = clahe.apply(l_channel)

        # merge the CLAHE enhanced L-channel with the a and b channel
        limg = cv2.merge((cl, a, b))

        # Converting image from LAB Color model to BGR color space
        enhanced_img = cv2.cvtColor(limg, cv2.COLOR_LAB2BGR)

        return enhanced_img

    @staticmethod
    def detect(img):
        """ Applies Canny edge detection to find key bounding boxes.
            Note:
                There is no postprocessing and the canny parameters are very general. The goal of whole this edge
                detection is to support the neural network detector. Results of this will be used to detect
                shift and space keys on qwerty layout if not detected (e.g. space might not have neither icon nor text
                ont it). This should just dump some bounding boxes which are to be processed elsewhere.

            Parameters
            -------
            img : np.array
                Cv2 loaded image

            Returns
            -------
            _ : list
                List of bounding boxes in format [x1, y1, x2, y2]
        """

        img = CannyDetector.apply_clahe(img)           # enhanced contrast helps to highlight edges
        img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)    # easier detection in grayscale
        img = cv2.bilateralFilter(img, 8, 30, 70)      # filters noise while keeping edges
        img = cv2.Canny(img, 25, 50)                   # convert to binary image with detected edges

        # find bounding boxes of edge contours
        contours, _ = cv2.findContours(img, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
        bboxes = []
        for contour in contours:
            x, y, w, h = cv2.boundingRect(contour)
            bboxes.append([x, y, x+w, y+h])

        return bboxes


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-s", "--src", help="path to directory with images", required=True)
    parser.add_argument("-d", "--dst", help="output path for processed detection results", required=True)
    args = parser.parse_args()
    os.makedirs(args.dst, exist_ok=True)

    for img_file in [x for x in os.listdir(args.src) if x.endswith(".png")]:

        image = cv2.imread(os.path.join(args.src, img_file))
        detections = CannyDetector.detect(image)

        chars = [x for x in detections]  # copy
        # filter bboxes outside expected region
        min_y = median(c[3] for c in chars)  # median should be from middle row under which we want the bboxes
        max_x = max(c[0] for c in chars) // 2
        detections = [bbox for bbox in detections if  # centers of the bboxes must be within boundaries
                      (bbox[1] + bbox[3]) / 2 > min_y and (bbox[0] + bbox[2]) / 2 < max_x]

        # filter smaller bboxes than character bboxes
        min_width = median(c[2] - c[0] for c in chars)
        min_height = median(c[3] - c[1] for c in chars)
        detections = [bbox for bbox in detections if
                      bbox[2] - bbox[0] > min_width and bbox[3] - bbox[1] > min_height]

        # filter too high bboxes
        max_height = (max(c[3] for c in chars) - min(c[1] for c in chars)) / 2
        detections = [bbox for bbox in detections if bbox[3] - bbox[1] < max_height]

        for bbox in detections:
            cv2.rectangle(image, (bbox[0], bbox[1]), (bbox[2], bbox[3]), (0, 255, 0), 1)

        cv2.imwrite(os.path.join(args.dst, img_file), image)
