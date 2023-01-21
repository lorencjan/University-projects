# File: query_C_extractor.py
# Solution: UPA - project
# Date: 10.11.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: Contains class that extracts data necessary for the second variant of query C to csv file.


import platform
import pandas as pd
from functools import reduce
from .data_convertor import DataConverter
from .utils import write_to_file


class QueryCExtractor:
    """ Queries task C data from db and stores them into csv file. """

    @classmethod
    def get_provider_fields(cls, num=20):
        """ Loads number (given by parameter) of most common provider
            fields of care from the latest dataset in descending order.
        """

        converter = DataConverter("UPA")
        collections = converter.get_collection_names()
        latest_providers_dataset = sorted([x for x in collections if "providers" in x])[-1]
        aggr = [
            {
                "$group": {
                    "_id": "$OborPece",
                    "count": {"$sum": 1}
                }
            },
            {
                "$match": {"_id": {"$ne": "-1"}}
            },
            {
                "$sort": {"count": -1}
            },
            {
                "$limit": num
            }
        ]

        df = converter.mongodb_to_df(latest_providers_dataset, aggr=aggr).rename(columns={"_id": "field"})
        fields = list(df["field"])

        # in 2019-11 was "veřejné lékárenství" renamed to "praktické lékárenství" which we missed earlier as there
        # was no indication of it -> need to load that field as well for later merge
        if "praktické lékárenství" in fields:
            fields.append("veřejné lékárenství")

        return fields

    @classmethod
    def load_data(cls):
        """ Loads fields counts quarterly. """

        converter = DataConverter("UPA")
        aggr = [
            {
                "$group": {
                    "_id": "$OborPece",
                    "count": {"$sum": 1}
                }
            },
            {
                "$match": {"$or": [{"_id": field} for field in cls.get_provider_fields()]}
            }
        ]

        # get collections by 3 months (quarterly)
        collections = [x for x in converter.get_collection_names() if "providers" in x]
        collections_reduced = [x for i, x in enumerate(sorted(collections, reverse=True)) if i % 3 == 0]

        dfs = [converter.mongodb_to_df(c, aggr=aggr).rename(columns={"_id": "field", "count": c.split("_")[-1][:-3]})
               for c in sorted(collections_reduced)]

        # rename "veřejné lékárenství" to "praktické lékárenství"
        dfs = [df.set_index("field").T.rename(columns={"veřejné lékárenství": "praktické lékárenství"}).T.reset_index()
               for df in dfs]

        return reduce(lambda df1, df2: pd.merge(df1, df2), dfs)

    @classmethod
    def run(cls):
        """ Creates basic query C csv file. """

        df = cls.load_data()
        df["field"] = df["field"].str.capitalize()
        df = df.sort_values(by="field").rename(columns={"field": "Obor péče"})
        encoding = "utf-8" if platform.system() == "Linux" else "windows-1250"
        write_to_file("csv", "query_C.csv", df.to_csv(index=False, line_terminator="\n", encoding=encoding))
