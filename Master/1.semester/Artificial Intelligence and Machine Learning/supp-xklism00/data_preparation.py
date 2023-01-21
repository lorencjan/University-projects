#!/usr/bin/env python3

# File: data_preparation.py
# Solution: SUI - project
# Date: 9.10.2021
# Authors: Jan Lorenc, Michal Kliš, Michal Sova, Jana Gregorová
# Faculty: Faculty of information technology VUT
# Description: Loads collected dataset and creates training and validation datasets.


import pickle
import numpy as np
import pandas as pd
from os import makedirs, listdir
from os.path import join, exists


class DataProcessor:

    raw_data_location = join("supp-xklism00", "raw_data")
    data_location = join("supp-xklism00", "data")

    df = pd.DataFrame({"BoardState": [], "Winner": []})
    training_df = None
    validation_df = None

    @classmethod
    def load_raw_data(cls):
        """ Loads raw dataset collected from the game. """

        files = [join(cls.raw_data_location, file) for file in listdir(cls.raw_data_location)]
        for file in files:
            try:  # pickle file can be broken
                with open(file, "br") as f:
                    cls.df = pd.concat([cls.df, pickle.load(file=f)])
            except:
                continue

    @classmethod
    def process_data(cls):
        """ Divides the collected dataset to training and validation data. Commonly used ratio is 80% to 20%.
            Data are shuffled first and equal size for all classes ares assured.
        """

        # make all classes of equal size (according to the smallest)
        min_class_size = cls.df.groupby("Winner").count().reset_index()["BoardState"].min()
        players = range(1, 5)
        shuffled_df = cls.df.sample(frac=1)
        equal_sized_datasets = [shuffled_df[shuffled_df["Winner"] == p][:min_class_size] for p in players]
        df = pd.concat(equal_sized_datasets)

        # split to training and validation datasets 80:20
        cls.training_df, cls.validation_df = np.array_split(df.sample(frac=1), [int(.8 * df.shape[0])])

    @classmethod
    def save_data(cls):
        """ Saves the processed dataset to appropriate location. """

        if not exists(cls.data_location):
            makedirs(cls.data_location)

        with open(join(cls.data_location, "training_df"), "wb") as file:
            pickle.dump(cls.training_df, file)

        with open(join(cls.data_location, "validation_df"), "wb") as file:
            pickle.dump(cls.validation_df, file)


if __name__ == "__main__":
    DataProcessor.load_raw_data()
    DataProcessor.process_data()
    DataProcessor.save_data()
