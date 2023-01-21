import numpy as np


class ReLU:

    @staticmethod
    def fn(x):
        """ The ReLU function. """

        return np.maximum(x, 0)

    @staticmethod
    def grad(x):
        """ Gradient of the ReLU function. """

        y = ReLU.fn(x)
        y[y >= 0] = 1
        return y
