import numpy as np


class CrossEntropy:

    @staticmethod
    def fn(y, d, epsilon=1e-15):
        """ Computes cross entropy of output 'y' in respect to desired 'd'. """

        m = d.shape[1]
        loss = -1 * (d * np.log(y + epsilon) + (1 - d) * np.log(1 - y + epsilon))
        return np.squeeze(1 / m * np.sum(loss))

    @staticmethod
    def grad(y, d):
        """ Computes error gradient from the output layer."""

        return -(np.divide(d, y) - np.divide(1 - d, 1 - y))
