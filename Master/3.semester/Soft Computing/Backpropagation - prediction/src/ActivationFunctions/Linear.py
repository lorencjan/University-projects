import numpy as np


class Linear:

    @staticmethod
    def fn(x):
        """ The linear function. """

        return x

    @staticmethod
    def grad(x):
        """ Gradient of the linear function. """

        return np.ones(x.shape)
