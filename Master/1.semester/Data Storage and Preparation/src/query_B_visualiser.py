# File: query_B_visualiser.py
# Solution: UPA - project
# Date: 8.11.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: Contains class that creates query B plot from prepared csv file. This script counts on its existence.


import pandas as pd
import seaborn as sns
from matplotlib import pyplot as plt
from .utils import save_fig


class QueryBVisualiser:
    """ Class responsible for Task B plot. """

    @classmethod
    def load_data(cls):
        """ Loads csv and prepares dataframes for plots.
            Returns first dataframe for bar plot and then for line plot.
        """

        df = pd.read_csv("csv/query_B.csv")
        df["peopleOnProvider"] = df["population"] / df["providers"]
        df = df.sort_values(by="peopleOnProvider", ascending=False)

        df2 = df.drop(columns=["peopleOnProvider"])
        df2 = pd.melt(df2, "region", ["population", "providers"])
        df2 = df2.rename(columns={"variable": "type", "value": "count"})

        return df, df2

    @classmethod
    def beautify_plot(cls, ax1, ax2):
        """ Sets titles, labels and legend """

        ax1.legend(labels=["Obyvatelé", "Lékaři", "Obyvatelé / lékař"], loc="upper right")
        legend = ax1.get_legend()
        legend.legendHandles[0].set_color('#61de47')
        legend.legendHandles[1].set_color('#de3737')
        legend.legendHandles[2].set_color('blue')

        ax1.set(yscale="log")  # needed due to the huge difference in counts of population and providers
        ax1.set_title("Žebříček krajů dle počtu obyvatel na praktické lékaře", fontsize=20)
        ax1.set_xlabel("Kraj")
        ax1.set_ylabel("Počet obyvatel a lékařů [log]")
        ax1.minorticks_off()

        ax2.set_ylabel("Počet obyvatel na 1 lékaře")
        ax2.grid(False)

    @classmethod
    def run(cls):
        """ Creates query B plot. """

        df_line, df_bar = cls.load_data()

        sns.set_style("darkgrid")
        fig, ax1 = plt.subplots(figsize=(10, 6))
        ax2 = ax1.twinx()
        sns.barplot(ax=ax1, data=df_bar, x="region", y="count", hue="type", palette=["#61de47", "#de3737"])
        sns.lineplot(ax=ax2, data=df_line, x="region", y="peopleOnProvider", marker="o")

        cls.beautify_plot(ax1, ax2)
        save_fig(plt, "query_B.png")
