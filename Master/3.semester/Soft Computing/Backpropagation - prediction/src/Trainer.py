import numpy as np
import pickle as pkl


class Trainer:
    """ Class responsible for training given model """

    def __init__(self, model, train_x, train_y, test_x, test_y):
        self.model = model
        self.loss_train, self.loss_test = 0, 0
        # neural net expects first dimension (features) and second count
        self.train_x, self.train_y, self.test_x, self.test_y = train_x.T, train_y.T, test_x.T, test_y.T

    def train(self, epochs=50, batch_size=5, verbose=False, save_filename=None):
        """ Performs training and validation of the model on provided data. """

        train_batches = self.train_x.shape[1] // batch_size + 1
        test_batches = self.test_x.shape[1] // batch_size + 1
        best_model = None
        best_loss = np.inf
        for epoch in range(epochs):
            self.loss_train, self.loss_test = 0, 0

            # training part
            for i in range(train_batches):
                train_x, train_y = self.get_batch(batch_size)
                y = self.model.forward(train_x)
                self.loss_train += self.model.criterion(y, train_y)
                self.model.backward(y, train_y, i + 1)

            # validation part
            for i in range(test_batches):
                test_x, test_y = self.get_batch(batch_size, phase="test")
                y = self.model.forward(test_x)
                self.loss_test += self.model.criterion(y, test_y)

            # report update
            self.loss_train /= train_batches
            self.loss_test /= test_batches
            if verbose:
                print(f"Train: Epoch: [{epoch + 1}/{epochs}] Loss: {self.loss_train}")
                print(f"Test: Epoch: [{epoch + 1}/{epochs}] Loss: {self.loss_test}")

            # save best model yet
            if self.loss_test < best_loss:
                best_loss = self.loss_test
                best_model = self.model

        if save_filename:
            with open(save_filename, "wb") as f:
                pkl.dump(best_model, f)

    def get_batch(self, batch_size, phase="train"):
        """ Creates random batches of given size out of the data specified by phase (train | test) """

        if phase == "train":
            start = np.random.randint(0, self.train_x.shape[1] - batch_size)
            end = start + batch_size
            return self.train_x[:, start:end], self.train_y[:, start:end]

        start = np.random.randint(0, self.test_x.shape[1] - batch_size)
        end = start + batch_size
        return self.test_x[:, start:end], self.test_y[:, start:end]
