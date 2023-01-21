import numpy as np
from ActivationFunctions.Linear import Linear
from LossFunctions.MSE import MSE
from NeuralNetwork.Layer import Layer


class NN:
    """ Represents the Neural Network structure """

    def __init__(self, loss=MSE, lr=0.01):
        """
        Parameters
        -------
        loss : class
            Loss function represented by a class from LossFunctions module
        lr : float
            Learning rate
        """

        self.layers = []
        self.loss = loss
        self.lr = lr

    def add_layer(self, dim=None, size=1, activation=Linear):
        """ Adds a layer to the network.

            Parameters
            -------
            dim : int
                Input dimension (number of features).
                If not specified, output dimension of previous layer is used.
                Must be specified for the first layer.
            size : int
                Number of neurons in the layer
            activation : class
                Activation function represented by a class from ActivationFunctions module
        """

        if dim is None:
            if len(self.layers) == 0:
                raise ValueError("Parameter dim must be specified for the first layer")

            dim = self.layers[-1].output_dimension()

        self.layers.append(Layer(dim, size, activation))

    def criterion(self, y, d):
        """ Computes loss for given real and target values

            Parameters
            -------
            y : np.array
                Real output of the network.
            d : np.array
                Target output of the network.

            Returns
            -------
            : np.array
                Loss value for given parameters
        """

        return self.loss.fn(y, d)

    def forward(self, x):
        """ Performs whole forward pass layer by layer.

            Parameters
            -------
            x : np.array
                Input vector

            Returns
            -------
            : np.array
                Output of the network
        """

        for layer in self.layers:
            x = layer.forward(x)

        return x

    def backward(self, y, d, t):
        """ Performs whole backward pass layer by layer.

            Parameters
            -------
            y : np.array
                Real output of the network.
            d : np.array
                Target output of the network.
            t : int
                Batch iteration for Adam optimizer.
        """

        grad = self.loss.grad(y, d)          # compute network error
        for layer in reversed(self.layers):  # pass and compute errors through layers
            grad = layer.backward(grad, self.lr, t)
