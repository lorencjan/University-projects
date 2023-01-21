#!/usr/bin/env python3.8
# coding=utf-8

from matplotlib import pyplot as plt
import pandas as pd
import seaborn as sns
import numpy as np
import os

pd.options.mode.chained_assignment = None

# Ukol 1: nacteni dat
def get_dataframe(filename: str, verbose: bool = False):
    """ Loads pandas dataframe from specified pickle file.
        Adds a date column to it and transform adequate types
        to categorical ones.
        Returns this modified dataset.
    """
    
    # gets dataframe from pickle
    df = pd.read_pickle(filename)
    
    # prints original size
    MB = 1048576
    if verbose:
        usage = df.memory_usage(deep=True, index=True).sum() / MB
        print("orig_size={:.1f} MB".format(usage))
    
    # adds date columns with proper type
    df["date"] = df["p2a"].astype("datetime64")
    
    # defines what shouldn't be categorical
    not_categoricals = ["p1", "p2a", "p2b", "p13a", "p13b", "p13c", "p53",
                        "a", "b", "d", "e", "f", "g", "j", "l", "n", "q",
                        "r", "region", "date"]
    
    # convert to categoricals
    for col in [x for x in df.head().columns if x not in not_categoricals]:
        df[col] = df[col].astype("category")
    
    # prints final size
    if verbose:
        usage = df.memory_usage(deep=True, index=True).sum() / MB
        print("new_size={:.1f} MB".format(usage))
    
    return df

# Ukol 2: následky nehod v jednotlivých regionech
def plot_conseq(df: pd.DataFrame, fig_location: str = None,
                show_figure: bool = False):
    """ Plots accidents severity graphs.
        Saves or displays it if proper parameters are set.
    """  
    
    # group selected columns by region
    cols = ["p1", "p13a", "p13b", "p13c", "region"]
    df = df[cols].groupby(["region"], as_index=False)
    
    # prepare data (aggregate, rename, sort, reformat)
    df = df.agg({"p13a":"sum", "p13b":"sum", "p13c":"sum", "p1":"count"})
    df.columns = ["region", "Úmrtí", "Těžce ranění",
                 "Lehce ranění", "Celkem nehod"]
    df = df.sort_values("Celkem nehod", ascending=False)
    df = df.melt(["region"])
    
    # plot
    sns.set_style("darkgrid")
    g = sns.catplot(data=df, x="region", y="value", row="variable",
                    kind="bar", sharey=False,
                    palette=sns.color_palette("Paired", 14))
    
    g.fig.set_size_inches(8.27, 11.69) # A4
    # remove x tick markers
    for ax in g.axes:
        ax[0].tick_params(axis='x', width=0)
    # set labels and titles
    g.set_axis_labels("", "Počet")
    g.set_xticklabels(df["region"].values.tolist())
    g.set_titles("{row_name}", size=12, fontweight="bold")
    
    plt.tight_layout()
        
    # save figure
    if fig_location != None:
        path = fig_location.rsplit('/', 1)
        if len(path) > 1 and not os.path.exists(path[0]):
            os.makedirs(path[0])
        plt.savefig(fig_location)
    
    # display plot
    if show_figure:
        plt.show()
    

# Ukol3: příčina nehody a škoda
def plot_damage(df: pd.DataFrame, fig_location: str = None,
                show_figure: bool = False):
    """ Plots causes and demages of accidents.
        Saves or displays it if proper parameters are set.
    """
    
    # work only with necessary columns and my specific 4 regions
    regions = ["PHA", "JHM", "JHC", "PLK"]
    cols = ["p12", "p53", "region"]
    df = df.loc[df["region"].isin(regions)][cols]
    
    # set categories and their names
    cause_labels = ["nezaviněná řidičem", "nepřiměřená rychlost jízdy",
                    "nesprávné předjíždění", "nedání přednosti v jízdě",
                    "nesprávný způsob jízdy", "technická závada vozidla"]
    cause_bins = [99, 200, 300, 400, 500, 600, 700]
    
    demage_labels = ["< 50", "50 - 200", "200 - 500", "500 - 1000", "> 1000"]
    demage_bins = [-1, 50, 200, 500, 1000, float('inf')]
    
    # format data
    region_data = []
    for region in regions:
        # categorize causes and demages
        x = df.loc[df["region"] == region]
        x["causes"] = pd.cut(x["p12"], cause_bins, labels=cause_labels)
        x["demages"] = pd.cut(x["p53"], demage_bins, labels=demage_labels)
        # create final dataset for the region
        grouped = x.groupby(["demages", "causes"], as_index=False)
        grouped = grouped.agg("count").rename(columns={"p12":"count"})
        grouped["region"] = region
        region_data.append(grouped.drop(columns=["p53"]))
    
    # put region data together
    df = pd.concat(region_data)
    
    # plot
    sns.set_style("darkgrid")
    g = sns.catplot(data=df, x="demages", y="count", hue="causes", kind="bar",
                    col="region", col_wrap=2, sharey=False, sharex=False,
                    legend_out=True, palette=sns.color_palette("Set3"))
    
    g.fig.set_size_inches(12, 8)
    # set y to log scale and labels to all plots, otherwise it'd be common
    for ax in g.axes:
        ax.set(yscale="log")
        ax.set_xlabel("Škoda [tisíc Kč]")
        ax.set_ylabel("Počet")
        ax.set_xticklabels(demage_labels)
    # set titles to plots and legend
    g.set_titles("{col_name}", size=12, fontweight="bold")
    g._legend.set_title("Příčina nehody")
    # tight layout in a way not to cover the legend
    plt.tight_layout(rect=[0, 0, 0.82, 1.0])
        
    # save figure
    if fig_location != None:
        path = fig_location.rsplit('/', 1)
        if len(path) > 1 and not os.path.exists(path[0]):
            os.makedirs(path[0])
        plt.savefig(fig_location)
    
    # display plot
    if show_figure:
        plt.show()
    

# Ukol 4: povrch vozovky
def plot_surface(df: pd.DataFrame, fig_location: str = None,
                 show_figure: bool = False):
    """ Plots accidents in time based on the surface conditions.
        Saves or displays it if proper parameters are set.
    """
    
    # work only with necessary columns and my specific 4 regions
    regions = ["KVK", "OLK", "PAK", "LBK"]
    cols = ["p1", "p16", "date", "region"]
    df = df.loc[df["region"].isin(regions)][cols]
    
    # create the contingency table
    df = pd.crosstab([df["region"], df["date"]], df["p16"])
    
    # rename p16 column values
    surfaces = {0: "jiný stav",
                1: "suchý neznečištěný",
                2: "suchý znečištěný",
                3: "mokrý",
                4: "bláto",
                5: "náledí, ujetý sníh - posypané",
                6: "náledí, ujetý sníh - neposypané",
                7: "rozlitý olej, nafta apod.",
                8: "souvislý sníh",
                9: "náhlá změna stavu"}
    df = df.rename(columns=surfaces)
    
    # cannot subsample to month with MultiIndex -> move region to column
    df = df.reset_index("region")
    # now I can subsample to month and at the same time group by region back
    df = df.groupby(["region", pd.Grouper(freq='M')]).agg(["sum"])
    # stack the surface and set it a name
    df = df.stack([0]).rename_axis(["region", "date", "surface"])
    
    # plot
    sns.set_style("darkgrid")
    g = sns.relplot(data=df, x="date", y="sum", hue="surface",
                    col="region", col_wrap=2, kind="line")
    
    g.fig.set_size_inches(15, 7)
    # set labels and titles
    g.set_axis_labels("Datum vzniku nehody", "Počet nehod")
    g.set_titles("{col_name}", size=12, fontweight="bold")
    g._legend.set_title("Stav vozovky")
    # tight layout in a way not to cover the legend
    plt.tight_layout(rect=[0, 0, 0.84, 1.0])
    
    # save figure
    if fig_location != None:
        path = fig_location.rsplit('/', 1)
        if len(path) > 1 and not os.path.exists(path[0]):
            os.makedirs(path[0])
        plt.savefig(fig_location)
    
    # display plot
    if show_figure:
        plt.show()
    
    

if __name__ == "__main__":
    # run all
    df = get_dataframe("accidents.pkl.gz", True)
    #plot_conseq(df, "01_nasledky.png", True)
    plot_damage(df, "02_priciny.png", True)
    #plot_surface(df, "03_stav.png", True)
