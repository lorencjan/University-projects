import matplotlib.pyplot as plt
import pickle as pkl

from DataLoader import DataLoader
from Trainer import Trainer
from ActivationFunctions.ReLU import ReLU
from ActivationFunctions.Sigmoid import Sigmoid
from NeuralNetwork.NN import NN


class Runner:
    """ Class responsible for running the prediction tasks. """

    @staticmethod
    def houses(train=True, model_path="trainedModels/housing.pkl"):
        """ Runs the task of predicting prices of Boston houses. Saves the data and model for the reference. """

        dataloader = DataLoader("data")

        if train:
            model = NN()
            model.add_layer(dim=13, size=64, activation=ReLU)
            model.add_layer(size=64, activation=ReLU)
            model.add_layer(size=1)

            trainer = Trainer(model, *dataloader.load_train_and_test(True))
            trainer.train(50, 5, True, model_path)
            loss = trainer.loss_test
        else:
            with open(model_path, "rb") as f:
                model = pkl.load(f)

            _, _, test_x, test_y = dataloader.load_train_and_test()
            loss = model.criterion(model.forward(test_x.T), test_y.T)

        print(f"Loss : {loss:.3f}")

    @staticmethod
    def houses_k_fold(k=10):
        """ Runs the task of predicting prices of Boston houses.
            Loads the dataset and trains & tests the model 'k' times.
            Prints the average test loss function to the standard output.
        """

        dataloader = DataLoader("data")
        dataset = dataloader.load_dataset()

        model = NN()
        model.add_layer(dim=13, size=13, activation=ReLU)
        model.add_layer(size=1)

        test_loss = 0
        for i in range(k):
            print(f"Fold run: {i + 1}/{k}", end="\r")
            trainer = Trainer(model, *dataloader.get_split_data(dataset))
            trainer.train()
            test_loss += trainer.loss_test

        print(f"Average loss in {k} folds: {(test_loss / k):.3f}")

    @staticmethod
    def approximate_sine_function(train=True, model_path="trainedModels/sinus.pkl"):
        """ This task attempts to approximate noisy sine function """

        train_x, train_y, test_x, test_y = DataLoader.generated_noisy_sinus()
        plt.scatter(train_x, train_y, s=0.25)

        if train:
            model = NN()
            model.add_layer(dim=1, size=16, activation=ReLU)
            model.add_layer(size=16, activation=Sigmoid)
            model.add_layer(size=1)

            trainer = Trainer(model, train_x, train_y, test_x, test_y)
            trainer.train(256, 8, True, model_path)
            model = trainer.model
        else:
            with open(model_path, "rb") as f:
                model = pkl.load(f)

        predicted_test_y = model.forward(test_x.T)
        plt.plot(test_x, predicted_test_y.T, c="#4ee44e")
        plt.legend(["Approximated sinus", "Noisy sinus"])
        plt.savefig("results/sinus.png")

    @staticmethod
    def linear_regression(train=True, model_path="trainedModels/linear_regression.pkl"):
        """ This task attempts to approximate noisy sine function """

        train_x, train_y, test_x, test_y = DataLoader.generated_noisy_linear_data()
        plt.scatter(train_x, train_y, s=0.25)

        if train:
            model = NN()
            model.add_layer(dim=1, size=64, activation=ReLU)
            model.add_layer(size=1)

            trainer = Trainer(model, train_x, train_y, test_x, test_y)
            trainer.train(64, 32, True, model_path)
            model = trainer.model
        else:
            with open(model_path, "rb") as f:
                model = pkl.load(f)

        predicted_test_y = model.forward(test_x.T)
        plt.plot(test_x, predicted_test_y.T, c="#4ee44e")
        plt.legend(["Predicted", "Noisy data"])
        plt.savefig("results/linear_regression.png")
