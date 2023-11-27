# File: detector_base.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Provides base detector class with common detector functionalities.

import sys
import math
import cv2
import numpy as np
import torch
import torchvision
from yolov7_helpers import *

sys.path.insert(0, "../yolov7")


class DetectorBase:
    """ Base class for keyboard and character detectors providing common functionality """

    def __init__(self, model_path, img_width=640, conf_threshold=0.5, iou_threshold=0):
        """ Parameters
            -------
            model_path : str
                Path to a trained model
            img_width : int
                Width of the image that the model takes as an input
            conf_threshold : float
                Detection confidence threshold for individual bounding boxes
            iou_threshold : float
                Allowed bounding box area overlap.
        """

        self.device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
        self.model = torch.load(model_path, map_location=self.device)["model"].float().fuse().eval()
        self.img_width = img_width
        self.conf_threshold = conf_threshold
        self.iou_threshold = iou_threshold

    def prepare_img(self, img):
        """ Takes cv2 image and transforms it to the detector's format.

            Parameters
            -------
            img : np.array
                OpenCV image.

            Returns
            -------
            _ : np.array
                Image in format to be detected by yolov7 detector.
        """

        # from datect.py (yolov7)
        stride = int(self.model.stride.max())                   # model stride
        img_size = math.ceil(self.img_width / stride) * stride  # check img_size

        # from dataloader
        img = letterbox(img, (img_size, img_size), stride=stride)[0]  # add padding to detector size
        img = img[:, :, ::-1].transpose(2, 0, 1)                      # BGR to RGB, to 3x416x416
        img = np.ascontiguousarray(img)
        img = torch.from_numpy(img).to(self.device).float()           # to torch
        img /= 255.0                                                  # normalize 0-255 to 0.0-1.0
        if img.ndimension() == 3:
            img = img.unsqueeze(0)

        return img

    def model_detect(self, images):
        """ Runs detection using trained model.

            Parameters
            -------
            images : list
                List of np images to run batched detection on (prepared with prepare_img method)

            Returns
            -------
            _ : list
                List of bounding boxes for each input image.
        """

        orig_img_shapes = [img.shape for img in images]
        images = torch.cat([self.prepare_img(img) for img in images])

        # cuda constructor tends to be quite slow and is good practice to init model with zeros
        if self.device.type != "cpu":
            zeros = torch.zeros(len(images), 3, self.img_width, self.img_width)
            self.model(zeros.to(self.device).type_as(next(self.model.parameters())))

        # inference
        with torch.no_grad():   # turn off gradient calculation
            pred = self.model(images)[0]

        # Apply NMS
        pred = non_max_suppression(pred, self.conf_threshold, self.iou_threshold)
        pred = [x for x in pred if len(x) > 0]  # filter valid ones

        # process detections - rescale bounding boxes back to original size
        for i, det in enumerate(pred):
            det[:, :4] = scale_coords(images[i].shape[1:], det[:, :4], orig_img_shapes[i]).round()

        return pred

    @staticmethod
    def convert_detections_to_json(detections, label_map):
        """ Takes raw tensor detection results and converts it to a json dictionary.

            Parameters
            -------
            detections : torch.tensor
                Raw tensor detection results
            label_map : dict
                Dictionary mapping numeric labels to their names

            Returns
            -------
            _ : dict
                Detection results as a json dict
        """

        json = {}
        for detection in detections:
            *bbox, conf, label = detection
            label_name = label_map[int(label.item())]
            value = {"bbox": torch.tensor(bbox).int().numpy().tolist(), "conf": round(conf.item(), 2)}
            if label_name in json.keys():
                json[label_name].append(value)
            else:
                json[label_name] = [value]
                
        return json
