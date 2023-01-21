import os.path
from os.path import join, exists
import numpy as np


class DataLoader:
    """ Class responsible for providing data for the regression tasks """

    def __init__(self, data_dir):
        self.delimiter = ","
        self.dataset_file = join(data_dir, "dataset.csv")
        self.train_dataset_file = join(data_dir, "train.csv")
        self.test_dataset_file = join(data_dir, "test.csv")

    def load_dataset(self):
        """ Loads and shuffles the main task dataset (Boston housing).
            Used if we want to work with the dataset further.
        """

        data = np.loadtxt(self.dataset_file, delimiter=self.delimiter)
        np.random.shuffle(data)

        return data

    def split_to_train_test(self, dataset, train_percentage=0.8):
        """ Splits the dataset into train and test datasets """

        train_size = int(dataset.shape[0] * train_percentage)
        train, test = dataset[:train_size, :], dataset[train_size:, :]

        return train, test

    def split_to_x_y(self, data, num_features=13):
        """ Last column in the dataset is target price,
            extract it to 'y' target set and leave feature 'x' data
        """

        x = data[:, 0:num_features]
        y = data[:, num_features]

        return x, y

    def load_train_and_test(self, load_new_and_save=False, train_percentage=0.8):
        """ Performs complete load, split and save action.
            Used if we want just a single load without anything else
        """

        if load_new_and_save or not exists(self.train_dataset_file):
            dataset = self.load_dataset()
            train, test = self.split_to_train_test(dataset, train_percentage)
            np.savetxt(self.train_dataset_file, train, delimiter=self.delimiter)
            np.savetxt(self.test_dataset_file, test, delimiter=self.delimiter)
        else:
            train = np.loadtxt(self.train_dataset_file, delimiter=self.delimiter)
            test = np.loadtxt(self.test_dataset_file, delimiter=self.delimiter)

        train_x, train_y = self.split_to_x_y(train)
        test_x, test_y = self.split_to_x_y(test)

        return train_x, self.reshape(train_y), test_x, self.reshape(test_y)

    def get_split_data(self, dataset):
        """ Performs the 2 actions of splitting dataset and extracting x and y parts. """

        train, test = self.split_to_train_test(dataset)
        train_x, train_y = self.split_to_x_y(train)
        test_x, test_y = self.split_to_x_y(test)

        return train_x, self.reshape(train_y), test_x, self.reshape(test_y)

    @staticmethod
    def generated_noisy_sinus():
        """ Generates sinus which we want to approximate in the task """

        # generate sinus for test data to which we want to approximate
        test_x = np.arange(-np.pi, np.pi, 0.005)
        test_y = np.sin(test_x)

        # train data is the same sinus but noisy
        train_x = test_x
        train_y = test_y + np.random.normal(0, 0.05, train_x.shape[0])

        d = DataLoader
        return d.reshape(train_x), d.reshape(train_y), d.reshape(test_x), d.reshape(test_y)

    @staticmethod
    def generated_noisy_linear_data():
        """ Generate linear data to approximate to and predict next """

        # generate linear test data to which we want to approximate + something extra to predict
        test_x = np.arange(0, 750)
        test_y = test_x

        # train data is the same but noisy
        train_x = test_x[:500]
        train_y = train_x + np.random.uniform(-50, 50, train_x.shape[0])

        d = DataLoader
        return d.reshape(train_x), d.reshape(train_y), d.reshape(test_x), d.reshape(test_y)

    @staticmethod
    def reshape(arr):
        """ Makes 2-dim array out of 1 dim array """

        return arr.reshape(arr.shape[0], 1)
