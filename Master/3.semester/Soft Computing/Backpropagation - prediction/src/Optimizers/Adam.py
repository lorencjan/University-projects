import numpy as np


class Adam:
    """ Represents Adam optimizer """

    def __init__(self, b1=0.9, b2=0.999, epsilon=1e-8):
        self.m_w, self.v_w = 0, 0
        self.m_b, self.v_b = 0, 0
        self.b1 = b1
        self.b2 = b2
        self.epsilon = epsilon

    def fn(self, w, b, t):
        """ Computes new weight and bias gradient using Adam optimization.

            Parameters
            -------
            w : np.array
                Weights gradient from backward phase
            b : np.array
                Bias gradient from backward phase
            t : int
                Batch iteration for power of beta in correction step

            Returns
            -------
            : tuple
                New weight and bias gradients
        """

        # compute momentum m for weights and biases
        self.m_w = self.b1 * self.m_w + (1 - self.b1) * w
        self.m_b = self.b1 * self.m_b + (1 - self.b1) * b

        # compute rms v for weights and biases
        self.v_w = self.b2 * self.v_w + (1 - self.b2) * w**2
        self.v_b = self.b2 * self.v_b + (1 - self.b2) * b**2

        # correction
        b1_denom = 1 - self.b1**t
        b2_denom = 1 - self.b2**t
        m_w = self.m_w / b1_denom
        m_b = self.m_b / b1_denom
        v_w = self.v_w / b2_denom
        v_b = self.v_b / b2_denom

        # compute new weight and bias gradients
        new_w = m_w / (np.sqrt(v_w) + self.epsilon)
        new_b = m_b / (np.sqrt(v_b) + self.epsilon)

        return new_w, new_b
