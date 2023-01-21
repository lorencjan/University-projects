#!/usr/bin/python3.8
# coding=utf-8

import os
import numpy as np
import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt

def get_dataframe(filename: str):
    """ Loads pandas dataframe from specified pickle file.
        Takes only necessary data, renames and transforms it.
        Returns this modified dataframe.
    """
    
    # get dataframe from pickle
    df = pd.read_pickle(filename)
    total_count = df.shape[0]
    # remove invalid values
    df = df[df["p49"]!=-1]
    # get only skids, conditions and ids for aggregation purposes
    df = df[["p1", "p18", "p49"]]
    df = df.rename(columns={"p1":"id", "p18":"condition", "p49":"skid"})
    # transform 0/1 for skids to booleans
    df["skid"] = df["skid"].astype(np.bool)
    
    return df, total_count


def print_table(df: str, col_names: list()):
    """ Prints a contingency table representing skid
        statistics for specific weather conditions.
    """

    ct = pd.crosstab(df["skid"], df["condition"])
    ct.columns = pd.Index(col_names, name="Povětrnostní podmínky")
    ct.index = pd.Index(["Ne", "Ano"], name="Smyk")
    print("Počet nehod se smykem / bez smyku za různých " \
          "povětrnostních podmínek:")
    print(ct)


def print_additional_stats(df: str, total_count: int):
    """Prints additional statistical data."""

    print("\nDalší zajímavé statistiky:")
    print("Nehod celkem: {}".format(total_count))
    print("Nehod se smykem: {}".format(df[df["skid"]].shape[0]))
    print("Nehod bez smyku: {}".format(df.shape[0] - df[df["skid"]].shape[0]))
    print("Smyk nerozpoznán u {} nehod, tedy v {:.2f}% případů.".format(
        total_count - df.shape[0],
        100 * (total_count - df.shape[0]) / total_count))


def create_plot(df: str, col_names: list(), fig_location: str):
    """ Creates a plot showing the probrability of skids
        under different weather conditions.
    """

    # aggregate data for plot purposes
    df = df.groupby(["condition", "skid"], as_index=False).agg("count")
    df = df.rename(columns={"id":"count"})

    # create a new dataset which is going to be plotted
    df_plot = pd.DataFrame()
    # for each condition compute skid probability
    for cond in set(df["condition"]):
        x = df[df["condition"] == cond]
        skid_count = x.loc[x["skid"], "count"].sum()
        total_count = x["count"].sum()
        df_plot[cond] = pd.Series([100 * skid_count / total_count], ["prob"])
    # rename int value columns to their respective names
    df_plot.columns = col_names
    # sort the dataset by highest skid probability
    df_plot = df_plot.sort_values("prob", 1, False)
    # transform for easier plotting
    df_plot = df_plot.T.reset_index()

    # set size and style of the plot
    plt.figure(figsize=(11, 7))
    sns.set_style("darkgrid")
    # create horizontal bar plot
    ax = sns.barplot(data=df_plot, x="prob", y="index",
                     palette=sns.color_palette("Blues_r", n_colors=14))
    # rename and resize fonts of labels
    ax.set_xlabel('Pravděpodobnost smyku [%]', fontsize=20)
    ax.set_ylabel('Povětrnostní podmínky', fontsize=20)
    for tick in ax.xaxis.get_major_ticks():
        tick.label.set_fontsize(16)
    for tick in ax.yaxis.get_major_ticks():
        tick.label.set_fontsize(16) 
    # set labels to the values
    for p in ax.patches:
        width = p.get_width()
        height = p.get_height()
        ax.text(width + 1, p.get_y() + height / 2, "{:1.2f}".format(width),
                ha = 'left', va = 'center', fontsize=14)

    plt.tight_layout()
    
    # save figure
    if fig_location != None:
        path = fig_location.rsplit('/', 1)
        if len(path) > 1 and not os.path.exists(path[0]):
            os.makedirs(path[0])
        plt.savefig(fig_location)


if __name__ == "__main__":
    df, total_count = get_dataframe("accidents.pkl.gz")
    col_names = ["Jiné ztížené", "Neztížené", "Mlha",
                 "Slabý déšť / mrholení", "Déšť", "Sněžení", 
                 "Námraza / náledí", "Nárazový vítr"]
    print_table(df, col_names)
    print_additional_stats(df, total_count)
    create_plot(df, col_names, "fig.png")
