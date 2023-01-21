# File: query_B_visualiser.py
# Solution: UPA - project
# Date: 8.11.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: Contains class that creates query A plots from prepared csv files.
#              This script counts on their existence.


import platform
import pandas as pd
import seaborn as sns
from matplotlib import pyplot as plt
from .utils import save_fig


class QueryAVisualiser:
    """ Class responsible for Task A plots. """

    @classmethod
    def load_data(cls):
        """ Loads csv and prepares dataframes for plots.
            Returns first dataframe for bar plot and second dataframe for line plot.
        """

        encoding = "utf-8" if platform.system() == "Linux" else "windows-1250"
        df1 = pd.read_csv("csv/query_A1.csv", encoding=encoding)
        df2 = pd.read_csv("csv/query_A2.csv", encoding=encoding)

        return df1, df2

    @classmethod
    def preprocess_data(cls, df1, df2):
        """ Csv files contain raw db data. This method preprocesses them for the visaliyations. """

        # Query 1
        df1 = df1.replace({"CZ0641": "Ostatni", "CZ0642": "Brno", "CZ0643": "Ostatni", "CZ0644": "Ostatni",
                           "CZ0645": "Ostatni", "CZ0646": "Ostatni", "CZ0647": "Ostatni"})

        # Sum by region code and field of care
        df1["OborPece"] = df1["OborPece"].str.capitalize()
        df1 = df1.groupby(["OkresKod", "OborPece"]).sum()
        df1 = df1.reset_index().sort_values(by="OborPece")

        # Query 2
        df2["OborPece"] = df2["OborPece"].str.capitalize()
        df2 = df2.sort_values(by=["OborPece", "Datum"])

        return df1, df2

    @classmethod
    def beautify_plot_a1(cls, ax):
        """ Sets titles, labels and legend. """

        legend = ax.legend(title="Oblast", labels=["Brno", "Ostatní"], fontsize=14)
        legend.get_title().set_fontsize(16)
        legend.legendHandles[0].set_color("#61de47")
        legend.legendHandles[1].set_color("#de3737")

        ax.set_title("Počet poskytovatelů v Brně a ostatních okresech JMK", fontsize=20)
        ax.set_xlabel("Počet poskytovatelů")
        ax.set_ylabel("Obor péče")

        ax.minorticks_off()

    @classmethod
    def beautify_plot_a2(cls, ax):
        """ Sets titles, labels and legend. """

        ax.legend(labels=[
            "Dentální hygienistka",
            "Endokrinologie a diabetologie",
            "Klinický psycholog",
            "Ortodoncie",
            "Otorinolaryngologie a chirurgie hlavy a krku"
            ],
            loc=(1.05, 0))
        ax.tick_params(axis='x', rotation=90)

        ax.set_title("Vývoj počtu poskytovatelů v čase", fontsize=20)
        ax.set_ylabel("Počet poskytovatelů")
        ax.set_xlabel("Datum")
        ax.minorticks_off()

    @classmethod
    def plot_a1(cls, df):
        """ Creates plot for query 1. """

        sns.set_style("darkgrid")
        fig, ax = plt.subplots(figsize=(16, 10))
        sns.barplot(
              ax=ax,
              y="OborPece",
              x='count',
              data=df,
              hue="OkresKod",
              palette=["#61de47", "#de3737"],
              saturation=.90)
        cls.beautify_plot_a1(ax)
        save_fig(plt, "query_A1.png")

    @classmethod
    def plot_a2(cls, df):
        """ Creates plot for query 2. """

        sns.set_style("darkgrid")
        fig, ax = plt.subplots(figsize=(16, 10))
        sns.lineplot(
              ax=ax,
              y="count",
              x='Datum',
              data=df,
              hue="OborPece",
              palette="Set2")
        cls.beautify_plot_a2(ax)
        save_fig(plt, "query_A2.png")

    @classmethod
    def run(cls):
        """ Creates query A plots. """

        df1, df2 = cls.load_data()
        df1, df2 = cls.preprocess_data(df1, df2)
        cls.plot_a1(df1)
        cls.plot_a2(df2)
