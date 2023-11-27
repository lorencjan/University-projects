# File: detection_visualiser.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: This script visualizes detected bounding boxes on appropriate images.

import os
from os.path import join
import cv2
import json
import argparse

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-a", "--annotations", help="path to a directory with detection results", required=True)
    parser.add_argument("-i", "--images", help="path to a directory with detected images", required=True)
    parser.add_argument("-c", "--confidence", help="show confidence scores", default=False)
    args = parser.parse_args()

    for file in [x for x in os.listdir(args.images) if x.endswith(".png")]:
        img = cv2.imread(join(args.images, file))
        with open(f"{join(args.annotations, file[:-4])}.json", "r") as f:
            annotations = json.load(f)

        for cls, bboxes in annotations.items():
            for bbox in bboxes:
                x1, y1, x2, y2 = bbox["bbox"]
                cv2.rectangle(img, (x1, y1), (x2, y2), (0, 255, 0), 1)
                text = f"{cls} ({bbox['conf']}%)" if args.confidence else cls
                cv2.putText(img, text, (x1, y1 - 3), cv2.FONT_HERSHEY_COMPLEX, 0.5, (0, 255, 0), 1)

        cv2.imshow(file, img)
        cv2.waitKey(0)
        cv2.destroyAllWindows()
