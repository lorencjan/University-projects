# File: query_A_extractor.py
# Solution: UPA - project
# Date: 7.11.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: Contains class that extracts data necessary for queries A to csv files.


import platform
import pandas as pd
from .data_convertor import DataConverter
from .utils import write_to_file


class QueryAExtractor:
    """ Queries task A data from db and stores them into csv files. """

    # 15 fields of care for A1
    Obor_pece_a1 = [
        "revmatologie", "ortodoncie", "neurologie",
        "urologie", "kardiologie", "Dentální hygienistka",
        "klinická stomatologie", "traumatologie", "chirurgie",
        "psychiatrie", "gynekologie a porodnictví", "pneumologie a ftizeologie",
        "dětská nefrologie", "Fyzioterapeut", "gastroenterologie"
    ]

    # 5 fields of care for A2
    Obor_pece_a2 = [
        "otorinolaryngologie a chirurgie hlavy a krku", "endokrinologie a diabetologie", "Dentální hygienistka",
        "ortodoncie", "Klinický psycholog"
    ]

    @classmethod
    def load_providers_data(cls):
        """ For query A1 loads 15 fields of care in JMK and then group by region code and fields of care
            For query A2 loads 5 fields of care  throughout history.
        """

        converter = DataConverter("UPA")
        collections = converter.get_collection_names()
        latest_provider_name = sorted([x for x in collections if "providers" in x])[-1]
        # A1
        aggr = [
            {
                "$match": {
                    "$and": [
                        {"KrajKod": "CZ064"},                     # we want only the JMK
                        {"OborPece": {"$in": cls.Obor_pece_a1}},  # filter 15 fields of care
                    ]
                }
            },
            {
                "$group": {  # group by field of care
                    "_id": {
                        "OkresKod": "$OkresKod",
                        "OborPece": "$OborPece"
                    },
                    "count": {"$sum": 1},
                },
            },
            {
                "$project": {  # project only necessary data
                    "_id": 0,
                    "OkresKod": "$_id.OkresKod",
                    "OborPece": "$_id.OborPece",
                    "count": 1
                }
            },
        ]
        # replace region code to two variant Brno or others
        df1 = converter.mongodb_to_df(latest_provider_name, aggr=aggr)

        # A2
        aggr = [
            {
              "$match": {
                "OborPece": {"$in": cls.Obor_pece_a2}
              }
            },
            {
                "$group": {
                    "_id": "$OborPece",
                    "count": {"$sum": 1}
                },
            }  
        ]
        df2 = pd.DataFrame(columns=["OborPece", "Datum", "count"])
        for c_name in collections:
            new = converter.mongodb_to_df(c_name, aggr=aggr).rename(columns={"_id": "OborPece"})
            new["Datum"] = c_name.split("_")[-1]
            df2 = pd.concat([df2, new], ignore_index=True)

        return df1, df2
  
    @classmethod
    def run(cls):
        """ Creates query A csv files. """

        df1, df2 = cls.load_providers_data()
        encoding = "utf-8" if platform.system() == "Linux" else "windows-1250"
        write_to_file("csv", "query_A1.csv", df1.to_csv(index=False, line_terminator="\n", encoding=encoding))
        write_to_file("csv", "query_A2.csv", df2.to_csv(index=False, line_terminator="\n", encoding=encoding))
