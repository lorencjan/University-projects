# File: neural_net.py
# Solution: SUI - project
# Date: 9.10.2021
# Authors: Jan Lorenc, Michal Kliš, Michal Sova, Jana Gregorová
# Faculty: Faculty of information technology VUT
# Description: Contains neural net class.


import torch.nn as nn


class NeuralNet(nn.Module):
    """ Class representing the neural net. """

    def __init__(self):
        super(NeuralNet, self).__init__()
        self.seq = nn.Sequential(
            nn.Linear(637, 128),    # input layer ... the game state vector has length 637
            nn.ReLU(),              # ReLU activation function
            nn.Dropout(0.5),        # to prevent overfitting ... 0.5 is a goo rule of thumb
            nn.Linear(128, 64),
            nn.ReLU(),
            nn.Dropout(0.5),
            nn.Linear(64, 32),
            nn.ReLU(),
            nn.Dropout(0.25),
            nn.Linear(32, 4),       # we want probability for each player -> output of size 4
            nn.Softmax(dim=1)       # and softmax for the probability == game score for the player
        )

    def forward(self, x):
        return self.seq(x)
