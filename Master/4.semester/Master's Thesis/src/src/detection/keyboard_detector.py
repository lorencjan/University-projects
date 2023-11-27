# File: keyboard_detector.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: This file provides a keyboard detector.

import sys
sys.path.insert(0, "../training")

from yolov7_annotation_convert_maps import label_to_keyboard_map
from detector_base import DetectorBase


class KeyboardDetector(DetectorBase):
    """ Class responsible for keyboard detection using a trained model """

    def __init__(self, model_path):
        super().__init__(model_path)

    def detect(self, img):
        """ Runs keyboard detection on an image.

           Parameters
           ----------
           img : np.array
               Image to run detection on

           Returns
           -------
           _ : dict
               List of bboxes with their confidence score for found keyboards
       """

        detections = self.model_detect([img])
        return self.convert_detections_to_json(detections[0], label_to_keyboard_map) if detections else {}
