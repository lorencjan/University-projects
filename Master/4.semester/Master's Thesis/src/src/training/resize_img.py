# File: resize_img.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Script that resizes images to a given resolution. YOLOv7 can operate over 1280x1280 or 640x640 images
#              and can accept images other size which are resized to these resolutions automatically. However, for
#              better data manipulation (smaller dataset) and lesser RAM requirements during training it's good to
#              resize beforehand. No validation is done, the script presumes that only images are in the source folder.

import os
from os.path import join
import cv2
import argparse

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-s", "--src", help="path to a directory with images to resize", required=True)
    parser.add_argument("-d", "--dst", help="output directory for resized images", required=True)
    parser.add_argument("-iw", "--width", type=int, help="width of the image in pixels", default=640)
    args = parser.parse_args()

    os.makedirs(args.dst, exist_ok=True)

    for file in os.listdir(args.src):
        img = cv2.imread(join(args.src, file))
        height = round(img.shape[0] * (args.width / img.shape[1]))
        img = cv2.resize(img, (args.width, height), interpolation=cv2.INTER_CUBIC)
        cv2.imwrite(join(args.dst, file), img)
