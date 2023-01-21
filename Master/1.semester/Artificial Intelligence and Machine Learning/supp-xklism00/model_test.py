#!/usr/bin/env python3

# File: data_preparation.py
# Solution: SUI - project
# Date: 9.10.2021
# Authors: Jan Lorenc, Michal Kliš, Michal Sova, Jana Gregorová
# Faculty: Faculty of information technology VUT
# Description: Tests the trained model on testing dataset.


import torch
import pickle
import numpy as np
from os.path import join
from dicewars.ai.xklism00.neural_net import NeuralNet


class ModelTester:

    data_location = join("supp-xklism00", "data")
    model_location = join("supp-xklism00", "model")
    test_data = None
    test_labels = None
    model = None

    @classmethod
    def load_data(cls):
        """ Loads testing dataset and transforms it to torch tensors. """

        with open(join(cls.data_location, "testing_df"), "br") as file:
            test_df = pickle.load(file=file)

        test_data, test_labels = (np.vstack(test_df["BoardState"]), test_df["Winner"].to_numpy() - 1)
        cls.test_data = torch.Tensor(test_data)
        cls.test_labels = torch.Tensor(test_labels)

    @classmethod
    def load_model(cls):
        """ Loads model to be tested. """

        cls.model = NeuralNet()
        cls.model.load_state_dict(torch.load(cls.model_location))
        cls.model.eval()

    @classmethod
    def test(cls):
        """ Executes the test. All game state vectors from testing dataset are predicted
            and compared with the actual labels. The result is then printed out.
        """

        outputs = cls.model(cls.test_data)
        _, pred_labels = torch.max(outputs, dim=1)
        correct_predictions = torch.sum(pred_labels == cls.test_labels).item()
        test_data_size = len(cls.test_data)
        accuracy = correct_predictions / float(test_data_size)

        stats = [correct_predictions, test_data_size, accuracy]
        print("Test results: {}/{} predicted correctly -> accuracy: {}".format(*stats))


if __name__ == "__main__":
    ModelTester.load_data()
    ModelTester.load_model()
    ModelTester.test()
