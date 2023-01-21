import numpy as np


class MSE:

    @staticmethod
    def fn(y, d):
        """ Computes quadratic loss of output 'y' in respect to desired 'd'. """

        loss = np.square(d - y)
        return np.squeeze(np.sum(loss) / d.shape[1])

    @staticmethod
    def grad(y, d):
        """ Computes error gradient from the output layer."""

        return -2 * (d - y)
