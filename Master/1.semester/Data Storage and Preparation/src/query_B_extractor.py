# File: query_B_extractor.py
# Solution: UPA - project
# Date: 7.11.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: Contains class that extracts data necessary for the first variant of query B to csv file.


import pandas as pd
from datetime import datetime
from .data_convertor import DataConverter
from .utils import write_to_file


class QueryBExtractor:
    """ Queries task B data from db and stores them into csv file. """

    # short names for regions
    region_code_name = {"CZ010": "PHA", "CZ020": "STC", "CZ031": "JHC", "CZ032": "PLK", "CZ041": "KVK",
                        "CZ042": "ULK", "CZ051": "LBK", "CZ052": "HKK", "CZ053": "PAK", "CZ063": "VYS",
                        "CZ064": "JHM", "CZ071": "OLK", "CZ072": "ZLK", "CZ080": "MSK"}

    # codes for regions from the provided codebook
    regions_districts = {
        3018: "CZ010", 3026: "CZ020", 3034: "CZ031", 3042: "CZ032", 3051: "CZ041", 3069: "CZ042", 3077: "CZ051",
        3085: "CZ052", 3093: "CZ053", 3107: "CZ063", 3115: "CZ064", 3123: "CZ071", 3131: "CZ072", 3140: "CZ080"
    }

    @classmethod
    def load_population_data(cls):
        """ Loads number of all people older that 20 for each region district from db.
            Then it groups the district counts into regions and returns dataframe with them.
        """

        converter = DataConverter("UPA")
        aggr = [
            {
                "$match": {
                    "$and": [
                            {"casref_do": datetime(2020, 12, 31)},   # we want only the latest year
                            {"vek_kod": {"$gte": 410020610025000}},  # age >= 20
                            {"pohlavi_kod": -1},                     # both men and women
                            {"vuzemi_kod": {"$gt": 3000}},           # just regions
                            {"vuzemi_kod": {"$lt": 4000}}
                    ]
                }
            },
            {
                "$group": {
                    "_id": "$vuzemi_kod",
                    "count": {"$sum": "$hodnota"}
                }
            }
        ]
        df = converter.mongodb_to_df("population", aggr=aggr).rename(columns={"count": "population"})
        regions = pd.DataFrame({"_id": cls.regions_districts.keys(), "region_code": cls.regions_districts.values()})
        return regions.merge(df).drop(columns="_id")

    @classmethod
    def load_providers_data(cls):
        """ Loads number of general medical practitioners for each region from db and returns dataframe with them. """

        converter = DataConverter("UPA")
        collections = converter.get_collection_names()
        latest_provider_name = sorted([x for x in collections if "providers" in x])[-1]
        aggr = [
            {
                "$match": {"OborPece": "všeobecné praktické lékařství"}
            },
            {
                "$group": {
                    "_id": "$KrajKodSidlo",
                    "count": {"$sum": 1}
                }
            }
        ]
        df = converter.mongodb_to_df(latest_provider_name, aggr=aggr)
        return df.rename(columns={"_id": "region_code", "count": "providers"})

    @classmethod
    def run(cls):
        """ Creates query B csv file. """

        regions = pd.DataFrame({"region": cls.region_code_name.values(), "region_code": cls.region_code_name.keys()})
        pop_reg_counts = cls.load_population_data()
        provider_reg_counts = cls.load_providers_data()
        df = regions.merge(pop_reg_counts.merge(provider_reg_counts))
        df = df.drop(columns="region_code")
        write_to_file("csv", "query_B.csv", df.to_csv(index=False, line_terminator="\n"))
