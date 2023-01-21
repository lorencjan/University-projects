#!/usr/bin/env python3

# File: get_stat.py
# Solution: IZV - project part 1
# Date: 28.10.2020
# Author: Jan Lorenc
# Faculty: Faculty of information technology VUT
# Description: File function for displaying annual
#              car accidents count for each region

import os
import argparse
import numpy as np
import matplotlib.pyplot as plt
from matplotlib.gridspec import GridSpec
from download import DataDownloader

def plot_stat(data_source, fig_location = None, show_figure = False):
    """ Gets the data provided by DataDownloader class and plots column graphs
        for each year showing the number of car accident in the specific
        regions, where each region (X axis) has it's own column """
    
    header, data = data_source
    date_index = header.index("Datum")
    region_index = header.index("Region")
    
    # Get years.
    years_data = data[date_index].astype("datetime64[Y]").astype(int) + 1970
    years = np.unique(years_data)
    
    # Prepare plot.
    fig = plt.figure(figsize=(8.27, 11.69)) # A4
    fig.suptitle("Počet nehod v krajích v posledních letech", fontsize=14)
    spec = GridSpec(ncols=1, nrows=len(years), figure=fig, hspace=0.9)
    
    # Generate subplots.
    for i, year in enumerate(years):
        # Get regions and their counts.
        indices = np.where(years_data == year)
        regions_data = np.take(data[region_index], indices).flatten()
        regions, region_counts = np.unique(regions_data, return_counts=True)
        # Create and design subplot.
        ax = fig.add_subplot(spec[i])
        ax.set_title("Rok " + str(year))
        ax.set_xlabel("Kraje")
        ax.set_ylabel("Počet nehod")
        ax.spines["top"].set_visible(False)
        ax.spines["right"].set_visible(False)
        ax.bar(regions, region_counts, color="#64E572")
        ax.set_yticks([x*5000 for x in range(0, 6)])
        # Annotate columns by position numbers.
        positions = sorted(region_counts, reverse=True)
        num_of_cols = len(regions)
        for i, y in enumerate(region_counts):
            pos = positions.index(y) + 1
            x_offset = 0.0065 * num_of_cols
            if pos >= 10:
                x_offset += 0.028 * (num_of_cols % 10 +1)
            ax.text(i - x_offset, y + 1000, pos)
    
    if fig_location != None:
        folder = fig_location.rsplit('/', 1)[0]
        if not os.path.exists(folder):
            os.makedirs(folder)
        plt.savefig(fig_location)
    
    if show_figure:
        plt.show()

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--fig_location", dest='fig_location', type=str,
        help="sets location where to store the graph figure")
    parser.add_argument(
        "--show_figure", dest='show_figure', action='store_true',
        help="if set, the graph figure will be displayed during runtime")
    args = parser.parse_args()
    data_source = DataDownloader().get_list(["PHA", "KVK", "JHC"])
    plot_stat(data_source, args.fig_location, args.show_figure)
