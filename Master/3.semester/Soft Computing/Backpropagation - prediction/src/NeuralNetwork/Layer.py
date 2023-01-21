import numpy as np
from ActivationFunctions.Linear import Linear
from Optimizers.Adam import Adam


class Layer:
    """ Defines fully connected layer. """

    def __init__(self, dim, size, activation=Linear):
        """
        Parameters
        -------
        dim : int
            Input dimension of the layer
        size : int
            Number of neurons in the layer
        activation : class
            Activation function represented by a class from ActivationFunctions module
        """

        self.optimizer = Adam()
        self.activation = activation
        self.weights = None
        self.bias = None
        self.prev_x = None  # previous input to the layer
        self.prev_u = None  # previous inner potentials of the neurons
        self.init_weights(dim, size)

    def init_weights(self, dim, size, mu=0, sigma=0.01):
        """ Initializes weights randomly from normal distribution defined by mean and sigma.

            Parameters
            -------
            dim : int
                Input dimension (number of features)
            size : int
                Number of neurons in the layer
            mu : float
                Mean of the normal distribution
            sigma : float
                Sigma of the normal distribution
        """

        self.weights = mu + np.random.randn(size, dim) * sigma
        self.bias = np.zeros([size, 1])

    def forward(self, x):
        """ Computes forward pass through the layer.

            Parameters
            -------
            x : np.array
                Layer input

            Returns
            -------
            : np.array
                Activation output
        """

        self.prev_x = x                          # save the input for backward pass
        u = np.dot(self.weights, x) + self.bias  # compute inner potentials
        self.prev_u = u                          # save the inner potentials for backward pass

        return self.activation.fn(u)             # return activated potentials

    def backward(self, prev_grad, lr, t):
        """ Computes backward pass through the layer.

            Parameters
            -------
            prev_grad : np.array
                Gradient (error) of the previous layer
            lr : float
                Learning rate
            t : int
                Batch iteration for Adam optimizer

            Returns
            -------
            : np.array
                Gradient to pass to the previous (next in the propagation) layer
        """

        # compute gradients
        grad = prev_grad * self.activation.grad(self.prev_u)
        n = self.prev_x.shape[1]
        weights_grad = np.dot(grad, self.prev_x.T) / n
        bias_grad = np.sum(grad, axis=1, keepdims=True) / n

        # update weights and biases using Adam optimizer
        w, b = self.optimizer.fn(weights_grad, bias_grad, t)
        self.weights = self.weights - lr * w
        self.bias = self.bias - lr * b

        return np.dot(self.weights.T, grad)  # gradient for the previous layer

    def output_dimension(self):
        """
        Returns
        -------
        : np.array
            Output dimension of current layer
        """

        return self.weights.shape[0]
