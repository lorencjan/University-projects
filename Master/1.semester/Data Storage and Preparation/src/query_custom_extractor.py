# File: query_custom_extractor.py
# Solution: UPA - project
# Date: 7.11.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: Contains class that extracts data necessary for the custom queries to csv files.


import platform
import pandas as pd
from .data_convertor import DataConverter
from .utils import write_to_file
from datetime import datetime
from dateutil.relativedelta import relativedelta


class QueryCustomExtractor:
    """ Queries custom queries data from db and stores them into csv file. """

    @classmethod
    def load_coordinates_of_dentists_in_JHM(cls):
        """ Loads dentists in JHM region. Then, it extracts gps coordinates from this data. """

        converter = DataConverter("UPA")
        collections = converter.get_collection_names()
        latest_provider_name = sorted([x for x in collections if "providers" in x])[-1]
        aggr = [
            {
                "$match": {
                    "$and": [
                        {"KrajKodSidlo": "CZ064"},        # JHM region
                        {"OborPece": "zubní lékařství"},  # dentists
                        {"GPS": {"$ne": "-1"}}            # only valid data
                    ]
                },
            },
            {
                "$project": {    # all we need is GPS, nothing more
                    "_id": 0,
                    "GPS": 1
                }
            }
        ]

        return converter.mongodb_to_df(latest_provider_name, aggr=aggr)
    
    @classmethod
    def load_type_of_equipment(cls, region, type_of_equipment):
        """ Loads type of equipment from entered region and count equipment for hundred thousand"""

        converter = DataConverter("UPA")
        collections = converter.get_collection_names()

        aggr1 = [
            {
                "$match": {
                    "$and": [
                        {"Kraj": region},
                        {"DruhZarizeni": type_of_equipment}
                    ]
                }
            },
            {
                "$group": {
                    "_id": {
                            "DruhZarizeni": "$DruhZarizeni",
                            "Kraj": "$Kraj"
                            },
                    "count": {"$sum": 1}
                },
            },
            {
                 "$project": {  # project only necessary data
                    "_id": 0,
                    "Kraj": "$_id.Kraj",
                    "DruhZarizeni": "$_id.DruhZarizeni",
                    "count": 1
                 }
            },
        ]

        aggr2 = [
            {
                "$match": {
                    "$and": [
                            {"pohlavi_kod": -1},  
                            {"vek_kod": -1},
                            {"vuzemi_txt":  region},  
                    ]
                }
            },
            {
                "$group": {
                    "_id": {
                            "vuzemi_txt": "$vuzemi_txt",
                            "casref_do": "$casref_do"
                            },
                    "count": {"$sum": "$hodnota"}
                }
            },
            {
                 "$project": {  # project only necessary data
                    "_id": 0,
                    "vuzemi_txt": "$_id.vuzemi_txt",
                    "casref_do": "$_id.casref_do",
                    "count": 1
                 }
            }
        ]

        # Load population data
        df = converter.mongodb_to_df("population", aggr=aggr2).rename(columns={"count": "population"})
        custom_df = pd.DataFrame()
        for c_name in collections:
            new = converter.mongodb_to_df(c_name, aggr=aggr1)
            # Skip missing and empty data frames
            if new.empty:
                continue

            new["Datum"] = c_name.split("_")[-1]
            date_time_obj = datetime.fromisoformat(c_name.split("_")[-1]) - relativedelta(years=1)
            pom = df.loc[df["casref_do"].dt.year == date_time_obj.year]
            # Convert to 100 000 inhabitants
            new["count"] = new["count"] / pom["population"].values[0] * 100000
            custom_df = pd.concat([custom_df, new], ignore_index=True)

        return custom_df

    @classmethod 
    def run(cls):
        """ Creates custom queries csv files. """

        encoding = "utf-8" if platform.system() == "Linux" else "windows-1250"

        # Custom query 1
        regions = ["Pardubický kraj", "Zlínský kraj", "Moravskoslezský kraj", "Středočeský kraj"]
        df = pd.concat([cls.load_type_of_equipment(region, "Domácí zdravotní péče") for region in regions])
        write_to_file("csv", "query_custom1.csv", df.to_csv(index=False, line_terminator="\n", encoding=encoding))

        # Custom query 2
        gps_df = cls.load_coordinates_of_dentists_in_JHM()
        write_to_file("csv", "query_custom2.csv", gps_df.to_csv(index=False, line_terminator="\n"))
