#!/usr/bin/python3.8
# coding=utf-8
import os
import numpy as np
import pandas as pd
import geopandas
import matplotlib.pyplot as plt
import contextily as ctx
import numpy as np
from sklearn.cluster import MiniBatchKMeans

def make_geo(df: pd.DataFrame):
    """ Cleans dataframe from invalid coordinates data.
        Converts dataframe to geopandas dataframe with Krovak projection.
    """
    
    # Remove invalid coordinates X (d) a Y (e)
    df = df[np.isnan(df["d"]) == False]
    df = df[np.isnan(df["e"]) == False]
   
    # Convert to geodataframe
    return geopandas.GeoDataFrame(
        df,
        geometry=geopandas.points_from_xy(df["d"], df["e"]),
        crs="epsg:5514")

def plot_geo(gdf: geopandas.GeoDataFrame, fig_location: str = None,
             show_figure: bool = False):
    """ Plots 2 graphs of accidents in JHM region according
        to location of the accident.
    """

    # Align map
    gdf = gdf.to_crs("epsg:3857")

    # Get the data to plot
    gdf = gdf[gdf["region"] == "JHM"]
    in_cities = gdf[gdf["p5a"] == 1]
    in_country = gdf[gdf["p5a"] == 2]

    # Setup plots
    fig, axes = plt.subplots(1, 2, figsize=(14, 6))
    in_cities.plot(ax=axes[0], markersize=1, color="r")
    in_country.plot(ax=axes[1], markersize=1, color="g")
                    
    axes[0].title.set_text("Nehody v JHM kraji: v obci")
    axes[1].title.set_text("Nehody v JHM kraji: mimo obec")
    for ax in axes:
        ax.set_ylim(6.205e6, 6.39e6)
        ax.set_xlim(1.73e6, 1.97e6)
        ax.axis("off")
        ctx.add_basemap(ax, crs=gdf.crs.to_string(), alpha=0.9,
                        source=ctx.providers.Stamen.TonerLite)
    
    plt.tight_layout()

    # Save figure
    if fig_location != None:
        path = fig_location.rsplit('/', 1)
        if len(path) > 1 and not os.path.exists(path[0]):
            os.makedirs(path[0])
        plt.savefig(fig_location)
    
    # Display plot
    if show_figure:
        plt.show()


def plot_cluster(gdf: geopandas.GeoDataFrame, fig_location: str = None,
                 show_figure: bool = False):
    """ Plots graph showing accidents in JHM region.
        The graph also shows the accidents as clusters
        according to their proximity and count.
    """
    
    # Align map
    gdf = gdf.to_crs("epsg:3857")

    # Get the data to plot
    gdf = gdf[gdf["region"] == "JHM"]

    # Cluster by coordinates
    coords = np.dstack([gdf.geometry.x, gdf.geometry.y]).reshape(-1, 2)
    db = MiniBatchKMeans(n_clusters=16).fit(coords)
    
    # Group data by clusters
    gdf["cluster"] = db.labels_
    gdf = gdf.dissolve(by="cluster", aggfunc={"p1": "count"})
    gdf = gdf.rename(columns={"p1": "count"})

    # Get cluster centroids to plot
    gdf_coords = geopandas.GeoDataFrame(geometry=geopandas.points_from_xy(
        db.cluster_centers_[:, 0], db.cluster_centers_[:, 1]))
    gdf_plot = gdf.merge(gdf_coords, left_on="cluster", right_index=True)
    gdf_plot = gdf_plot.set_geometry("geometry_y")
    
    # Plot the clusters and the accidents
    fig, ax = plt.subplots(1, 1, figsize=(12, 7))
    gdf.plot(ax=ax, markersize=0.25, color="tab:gray", alpha=0.5)
    gdf_plot.plot(ax=ax, markersize=gdf_plot["count"] / 4, column="count",
                  legend=True, alpha=0.6)
    
    # Design the plot
    ax.title.set_text("Nehody v JHM kraji")
    ax.set_ylim(6.205e6, 6.39e6)
    ax.set_xlim(1.73e6, 1.97e6)
    ax.axis("off")
    ctx.add_basemap(ax, crs=gdf.crs.to_string(), alpha=0.9,
                    source=ctx.providers.Stamen.TonerLite)
    plt.tight_layout()
    
    # Save figure
    if fig_location != None:
        path = fig_location.rsplit('/', 1)
        if len(path) > 1 and not os.path.exists(path[0]):
            os.makedirs(path[0])
        plt.savefig(fig_location)
    
    # Display plot
    if show_figure:
        plt.show()


if __name__ == "__main__":
    gdf = make_geo(pd.read_pickle( "accidents.pkl.gz" ))
    plot_geo(gdf, "geo1.png", False)
    plot_cluster(gdf, "geo2.png", False)
