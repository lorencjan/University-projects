#!/usr/bin/env python3

# File: model_training.py
# Solution: SUI - project
# Date: 9.10.2021
# Authors: Jan Lorenc, Michal Kliš, Michal Sova, Jana Gregorová
# Faculty: Faculty of information technology VUT
# Description: Contains neural net model training.


import torch
import torch.nn as nn
from torch.utils.data import TensorDataset, DataLoader
from dicewars.ai.xklism00.neural_net import NeuralNet
import pickle
import numpy as np
from os.path import join


class NetTrainer:
    """ Takes care of the NN model training. """

    device = "cuda" if torch.cuda.is_available() else "cpu"
    training_data = None
    validation_data = None
    batch_size = 32
    validation_batch_size = 8
    epochs = 128
    model = NeuralNet().to(device)
    save_model_loc = join("supp-xklism00", "model")

    @classmethod
    def load_data(cls):
        """ Loads training + validation datasets and transforms them into DataLoaders for the training. """

        data_dir = join("supp-xklism00", "data")
        with open(join(data_dir, "training_df"), "br") as file:
            train_df = pickle.load(file=file)

        with open(join(data_dir, "validation_df"), "br") as file:
            val_df = pickle.load(file=file)

        # !!! beware !!! 4 classes (winners) are 1-4 as players, labels need to be 0-3
        # it's kind of one-hot, but we already have numeric labels -> just need to add it back in prediction
        train_data, train_labels = (np.vstack(train_df["BoardState"]), train_df["Winner"].to_numpy() - 1)
        val_data, val_labels = (np.vstack(val_df["BoardState"]), val_df["Winner"].to_numpy() - 1)
        train_dataset = TensorDataset(torch.Tensor(train_data), torch.LongTensor(train_labels))
        val_dataset = TensorDataset(torch.Tensor(val_data), torch.LongTensor(val_labels))
        cls.training_data = DataLoader(train_dataset, batch_size=cls.batch_size, shuffle=True)
        cls.validation_data = DataLoader(val_dataset, batch_size=cls.validation_batch_size, shuffle=True)

    @classmethod
    def train(cls):
        """ Trains the network. It uses CrossEntropy loss function and
            SGD optimizer with learning rate 0.001 -> needs more epochs.
            It prints the training process at the end of each epoch.
        """

        criterion = nn.CrossEntropyLoss()
        optimizer = torch.optim.SGD(cls.model.parameters(), lr=0.01)
        batch_iterations = len(cls.training_data)
        for epoch in range(1, cls.epochs + 1):
            if epoch == 24:
                optimizer = torch.optim.SGD(cls.model.parameters(), lr=0.005)
            if epoch == 64:
                optimizer = torch.optim.SGD(cls.model.parameters(), lr=0.001)
            loss_acc = 0
            accuracy_acc = 0
            for batch_data, batch_labels in cls.training_data:
                outputs = cls.model(batch_data)          # forward pass

                loss = criterion(outputs, batch_labels)  # compute loss
                loss_acc += loss.item()

                optimizer.zero_grad()                    # backpropagation
                loss.backward()

                optimizer.step()                         # improve net

                # compute ratio of correct label predictions
                _, pred_labels = torch.max(outputs, dim=1)  # we don"t need max scores now
                accuracy_acc += torch.sum(pred_labels == batch_labels).item() / float(cls.batch_size)

            stats = [epoch, cls.epochs, loss_acc / batch_iterations, accuracy_acc / batch_iterations]
            print("\n Train: Epoch: [{}/{}] Loss: {} Acc: {}".format(*stats))
            cls.validate(criterion, epoch)

        torch.save(cls.model.state_dict(), cls.save_model_loc)

    @classmethod
    def validate(cls, criterion, epoch):
        """ Validates the model for an epoch.
            Parameters
            ----------
            criterion :
                Loss function to compute the validation loss.
            epoch :
                Number of the current epoch to print.
        """

        model = cls.model.eval()
        loss_acc = 0
        accuracy_acc = 0
        batch_iterations = len(cls.validation_data)
        for batch_data, batch_labels in cls.validation_data:
            outputs = model(batch_data)
            loss_acc += criterion(outputs, batch_labels).item()

            _, pred_labels = torch.max(outputs, dim=1)
            accuracy_acc += torch.sum(pred_labels == batch_labels).item() / float(cls.validation_batch_size)

        stats = [epoch, cls.epochs, loss_acc / batch_iterations, accuracy_acc / batch_iterations]
        print(" Valid: Epoch: [{}/{}] Loss: {} Acc: {}".format(*stats))


if __name__ == "__main__":
    NetTrainer.load_data()
    NetTrainer.train()
