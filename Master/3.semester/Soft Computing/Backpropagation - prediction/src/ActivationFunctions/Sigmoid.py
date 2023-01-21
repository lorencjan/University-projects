import numpy as np


class Sigmoid:

    @staticmethod
    def fn(x):
        """ The sigmoid function. """

        return 1 / (1 + np.exp(-x))

    @staticmethod
    def grad(x):
        """ Gradient of the sigmoid function. """

        y = Sigmoid.fn(x)
        return y * (1 - y)
