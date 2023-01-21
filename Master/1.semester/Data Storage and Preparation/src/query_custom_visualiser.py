# File: query_custom_visualiser.py
# Solution: UPA - project
# Date: 7.11.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: Contains class that creates custom queries plots from prepared csv files.
#              This script counts on their existence.


import platform
import numpy as np
import pandas as pd
import geopandas as gpd
import contextily as ctx
import matplotlib.pyplot as plt
from pyproj import Transformer
from sklearn.cluster import DBSCAN
from .utils import save_fig
import seaborn as sns 


class QueryCustomVisualiser:
    """ Class responsible for custom queries plots. """

    @classmethod
    def plot_dentists_on_JHM_map(cls):
        """ Loads gps coordinates of dentists in JHM region and processes them for plotting purposes.
            Then, it creates a chart with two plots, one with exact dentist locations and one to
            visualise their distribution in the region. """

        # Prepare data for plotting
        gps_raw = pd.read_csv("csv/query_custom2.csv")
        gps_split = [{"x": x[0], "y": x[1]} for x in [x.split() for x in gps_raw["GPS"]]]
        gps = pd.DataFrame(gps_split)
        gdf = cls.transform_gps_to_krovak(gps)
        gdf_clusters = cls.cluster_dentist_locations(gdf)

        # Plot
        sns.reset_defaults()
        fig, axes = plt.subplots(1, 2, figsize=(16, 6), gridspec_kw={'width_ratios': [4, 5]})
        gdf.plot(ax=axes[0], markersize=0.7, color="b", legend=True)  # first plot with locations (those are left)
        gdf.plot(ax=axes[1], markersize=0.25, color="tab:gray", alpha=0.5)  # in cluster plot as well for suggestions)
        gdf_clusters.plot(ax=axes[1], markersize=gdf_clusters["count"]*6, column="count",  # second plot with clusters
                          legend=True, legend_kwds={"shrink": 0.85}, alpha=0.5)
        axes[0].title.set_text("Zubní ordinace v JHM")
        axes[1].title.set_text("Oblasti dostupnější zubní lékařské péče")
        for ax in axes:
            ax.set_ylim(6.205e6, 6.39e6)  # set map boundaries
            ax.set_xlim(1.73e6, 1.97e6)
            ax.axis("off")
            ctx.add_basemap(ax, crs=gdf.crs.to_string(), alpha=0.9, source=ctx.providers.Stamen.TonerLite)

        save_fig(plt, "query_custom2.png")

    @classmethod
    def transform_gps_to_krovak(cls, gps):
        """ Transforms pandas dataframe with standard wgs84 gps coordinates
            to Krovak's coordinates which are better for Czech republic.
            Returns geopandas dataframe with transformed coordinates.
        """

        x = gps["x"].to_numpy()
        y = gps["y"].to_numpy()
        transform_from = "epsg:4326"   # wgs84 (standard gps)
        transform_to = "epsg:5514"     # s-jtsk (Krovak)
        transformer = Transformer.from_crs(transform_from, transform_to)
        krovak_x, krovak_y = transformer.transform(x[:], y[:])
        df = pd.DataFrame({"x": krovak_x, "y": krovak_y})
        gdf = gpd.GeoDataFrame(df, geometry=gpd.points_from_xy(df["x"], df["y"]), crs="epsg:5514")
        gdf = gdf.to_crs("epsg:3857")  # align map

        return gdf

    @classmethod
    def cluster_dentist_locations(cls, dentist_coords):
        """ Clusters dentists by location using DBSCAN algorithm and returns dataframe with cluster coordinates. """

        # Perform clustering ... DBSCAN is suitable as it clusters by density
        coords = np.dstack([dentist_coords.geometry.x, dentist_coords.geometry.y]).reshape(-1, 2)
        clusters = DBSCAN(eps=5000, min_samples=5).fit(coords)

        # Create dataframe for the clustering results
        gdf = dentist_coords.copy()
        gdf["count"] = 0
        gdf["cluster"] = clusters.labels_
        gdf = gdf[gdf["cluster"] >= 0]

        # Compute centroids for the cluster (what is going to be plotted)
        centroids = np.zeros((1, 2), np.int)
        for cluster in gdf["cluster"].drop_duplicates():
            tmp = gdf[gdf["cluster"] == cluster].geometry
            centroids = np.concatenate((centroids, [[tmp.x.mean(), tmp.y.mean()]]))
        centroids = centroids[1:]  # remove the leading zeros header
        gdf_centroids = gpd.GeoDataFrame(geometry=gpd.points_from_xy(centroids[:, 0], centroids[:, 1]))

        # Group dataframe by clusters and add the centroids
        gdf = gdf.dissolve(by="cluster", aggfunc={"count": "count"})  # 'group by' for coordinates
        gdf = gdf.merge(gdf_centroids, left_on="cluster", right_index=True)
        gdf = gdf.set_geometry("geometry_y")

        return gdf

    @classmethod
    def plot_home_health_care(cls):
        """ Plot home health care for hundred thousand inhabitants. """

        encoding = "utf-8" if platform.system() == "Linux" else "windows-1250"
        df = pd.read_csv("csv/query_custom1.csv", encoding=encoding)
        df = df.sort_values(by=["Kraj", "Datum"])
        region_codes = df["Kraj"].unique()

        sns.set_style("darkgrid")
        # Create 2x2 matrix subplot
        fig, axes = plt.subplots(2, 2, figsize=(14, 12), sharex=True, sharey=True)
        ax = axes.flatten()
        for i in range(0, 4):
            sns.lineplot(ax=ax[i], y="count", x="Datum", data=df[df["Kraj"] == region_codes[i]])
            ax[i].set_title(region_codes[i])
            ax[i].tick_params(axis="x", rotation=75)
            ax[i].minorticks_off()
        
        # Set labels text and positon
        ax[0].set_ylabel("Počet")
        ax[2].set_ylabel("")
        ax[2].set_xlabel("")

        field_name = df["DruhZarizeni"][0].lower()
        fig.suptitle("Počet poskytovatelů " + field_name + " na 100 000 obyvatel v čase od roku 2018", size=18)

        # Set layout
        fig.tight_layout()
        plt.subplots_adjust(top=0.925, bottom=0.15, left=0.05)
        ax[0].yaxis.set_label_coords(-0.06, -0.05)
        ax[3].xaxis.set_label_coords(-0.015, -0.3)

        save_fig(plt, "query_custom1.png", tight_layout=False)

    @classmethod
    def run(cls):
        """ Creates custom queries csv files. """

        cls.plot_home_health_care()
        cls.plot_dentists_on_JHM_map()
