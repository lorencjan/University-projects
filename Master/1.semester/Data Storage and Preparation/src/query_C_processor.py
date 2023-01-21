# File: query_C_extractor.py
# Solution: UPA - project
# Date: 10.11.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: Contains class that extracts data necessary for the second variant of query C to csv file.


import os
import platform
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
from .utils import *


class QueryCProcessor:
    """ Processes task C data csv file. """

    data = None
    z_score_limit = 3  # for normal distribution
    z_score_normalization_constant = 0.6745
    mad = None
    regression_model = None
    regression_expected_y = None
    plot_dir = os.path.join("img", "C")

    @classmethod
    def load_data(cls):
        """ Loads fields counts quarterly. """

        encoding = "utf-8" if platform.system() == "Linux" else "windows-1250"
        cls.data = pd.read_csv("csv/query_C.csv", encoding=encoding)
        cls.data.set_index("Obor péče", inplace=True)

    @classmethod
    def plot_data(cls, fig, x, y, name, show_lines):
        """ Creates scatter plot for data and regression + z-score lines. """

        fig.axes[0].set_title(name, fontsize=20)
        plt.scatter(x, y)
        if show_lines:
            z_score_offset = cls.z_score_limit * cls.mad / cls.z_score_normalization_constant
            line = np.linspace(0, len(cls.data.columns), 100)
            plt.plot(line, cls.regression_model(line) + z_score_offset, c="r")
            plt.plot(line, cls.regression_model(line), c="g")
            plt.plot(line, cls.regression_model(line) - z_score_offset, c="r")

        plt.xticks(range(0, len(cls.data.columns)), labels=list(cls.data.columns), rotation=90)

    @classmethod
    def detect_outliers(cls, col, polynomial_degree=4):
        """ Detects outliers using polynomial regression and modified z-score method.
            Returns dataframes containing valid data and outliers.
        """

        real_y = cls.data.T[col].to_numpy()
        input_x = np.array(range(0, len(real_y)))

        cls.regression_model = np.poly1d(np.polyfit(input_x, real_y, polynomial_degree))
        cls.regression_expected_y = [cls.regression_model(x) for x in input_x]
        cls.mad = np.median(np.absolute(real_y - cls.regression_expected_y))
        abs_diffs = [np.abs((real_y[i] - cls.regression_expected_y[i])) for i in range(0, len(real_y))]
        modified_z_scores = [cls.z_score_normalization_constant * abs_diff / cls.mad for abs_diff in abs_diffs]

        df = pd.DataFrame({"x": input_x, "y": real_y, "zscore": modified_z_scores})
        valid_data = df[df["zscore"] <= cls.z_score_limit]
        outliers = df[df["zscore"] > cls.z_score_limit]

        return valid_data, outliers

    @classmethod
    def correct_outliers(cls, valid_data, outliers, col):
        """ Replaces outlier values with those expected by regression model. """

        outliers["y"] = [cls.regression_expected_y[x] for x in outliers["x"]]
        df = cls.data.T
        df[col] = list(pd.concat([valid_data, outliers]).sort_values(by=["x"])["y"].astype("int64"))
        cls.data = df.T

    @classmethod
    def plot_outlier_detection(cls, fig, valid_data, outliers, name):
        """ Saves plots displaying outlier detection. """

        plt.scatter(outliers["x"], outliers["y"], c="r")
        cls.plot_data(fig, valid_data["x"], valid_data["y"], name, True)

        plot_dir = os.path.join(cls.plot_dir, "outlier_detection")
        save_fig(plt, name, plot_dir)
        plt.cla()

    @classmethod
    def plot_corrected_outliers(cls, fig, name):
        """ Saves plots displaying data after correcting outliers. """

        y = cls.data.T[name].to_numpy()
        x = np.array(range(0, len(cls.data.columns)))

        cls.plot_data(fig, x, y, name, True)

        plot_dir = os.path.join(cls.plot_dir, "outlier_detection")
        save_fig(plt, name + "_corrected", plot_dir)
        plt.cla()

    @classmethod
    def run_outlier_detection(cls, plot_outlier_detection):
        """ Runs outlier detection, correction and visualises the process. """

        fig = plt.figure(figsize=(12, 9))
        for col in cls.data.T.columns:
            valid_data, outliers = cls.detect_outliers(col)
            if plot_outlier_detection:
                cls.plot_outlier_detection(fig, valid_data, outliers, col)

            cls.correct_outliers(valid_data, outliers, col)
            if plot_outlier_detection:
                cls.plot_corrected_outliers(fig, col)

    @classmethod
    def run_normalization(cls, new_min=0, new_max=1, field="Neurologie", plot=False):
        """ Normalizes a field using Min-Max normalization. """

        df = cls.data.T
        old_min = df[field].min()
        old_max = df[field].max()
        df[field] = new_min + (new_max - new_min) * (df[field] - old_min) / (old_max - old_min)

        if plot:
            fig = plt.figure(figsize=(12, 9))
            fig.gca()
            plot_dir = os.path.join(cls.plot_dir, "normalization")
            x = np.array(range(0, len(cls.data.columns)))

            cls.plot_data(fig, x, cls.data.T[field], field, False)
            save_fig(plt, field, plot_dir)
            plt.cla()

            cls.plot_data(fig, x, df[field], field, False)
            save_fig(plt, field + "_normalized", plot_dir)

        cls.data = df.T

    @classmethod
    def run_discretization(cls, bins=4, field="Psychiatrie", plot=False):
        """ Discretize values using equal-depth binning with median. """

        df = cls.data.T[[field]]
        df["bin"] = pd.qcut(df[field], bins, labels=False)
        grouped = df.groupby("bin").agg("median")
        df["discretized"] = [grouped[field][i] for i in df["bin"]]

        if plot:
            fig = plt.figure(figsize=(12, 9))
            fig.gca()
            plot_dir = os.path.join(cls.plot_dir, "discretization")
            x = np.array(range(0, len(cls.data.columns)))

            cls.plot_data(fig, x, cls.data.T[field], field, False)
            save_fig(plt, field, plot_dir)
            plt.cla()

            cls.plot_data(fig, x, df["discretized"], field, False)
            save_fig(plt, field + "_discretized", plot_dir)

        df2 = cls.data.T
        df2[field] = df["discretized"]
        cls.data = df2.T

    @classmethod
    def run(cls, save_process_plots=False):
        """ Creates processed query C csv file. """

        cls.load_data()
        cls.run_outlier_detection(save_process_plots)
        cls.run_normalization(plot=save_process_plots)
        cls.run_discretization(plot=save_process_plots)

        encoding = "utf-8" if platform.system() == "Linux" else "windows-1250"
        csv = cls.data.to_csv(line_terminator="\n", encoding=encoding)
        write_to_file("csv", "query_C_processed.csv", csv)
