#!/usr/bin/env python3

# File: downloader.py
# Solution: UPA - project part 1
# Date: 5.10.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: File containing class DataDownloader for downloading data


import requests
import platform
import pandas as pd
from os import listdir
from os.path import isfile, exists, join
from io import StringIO
from bs4 import BeautifulSoup
from .utils import write_to_file


class DataDownloader:
    """
    A class responsible for downloading data about 
    healthcare providers and czech population.
    """    
        
    def __init__(self):
        """ Sets data source url addresses, defines directory where to
            optionally store data and creates headers for requests
            so that the script does not look like a robot. """
        
        self.url_healthcare_providers_base = "https://nrpzs.uzis.cz/"
        self.url_healthcare_providers_page = self.url_healthcare_providers_base + "index.php?pg=home--download&archiv=sluzby"
        self.url_population = "https://www.czso.cz/documents/62353418/143522504/130142-21data043021.csv/760fab9c-d079-4d3a-afed-59cbb639e37d?version=1.1"
        self.save_dir = "data"

        self.request_headers = {
            "Accept-Encoding": "csv, deflate, br",
            "Accept-Language": "cs-CZ",
            "Accept": "text/csv",
            "Content-Type": "application/json; charset=UTF-8",
            "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) \
                           AppleWebKit/537.36 (KHTML, like Gecko) \
                           Chrome/85.0.4183.121 \
                           Safari/537.36 OPR/71.0.3770.228",
            "X-Requested-With": "XMLHttpRequest"
        }

    def get_healthcare_providers_data(self, save_data = False):
        """ Downloads all files containing data about healthcare providers
            from nrpzs.uzis.cz site and returns them as pandas dataframes.
        """
        
        # For development reasons so that the data doesn't get downloaded 
        # all the time, we download once and then load from them
        save_dir = join(self.save_dir, "healthcare_providers")
        if save_data and exists(save_dir):
            files = [f for f in listdir(save_dir) if isfile(join(save_dir, f))]          
            if len(files) > 0:
                dfs = dict()
                for f in files:
                    encoding = "utf-8" if platform.system() == "Linux" else "windows-1250"
                    df = pd.read_csv(join(save_dir, f), sep=";", encoding=encoding, low_memory=False)
                    dfs[f.replace(".csv", "")] = df

                return dfs

        # Get csv files' location on the website
        page = requests.get(self.url_healthcare_providers_page, headers = self.request_headers)
        soup = BeautifulSoup(page.content, "html.parser")
        links = [a["href"] for a in soup.select("#body-inner .row a")]
                
        # Download and transform to dataframes
        dfs = dict()
        for link in links:
            response = requests.get(self.url_healthcare_providers_base + link, headers = self.request_headers)
            converted_response = str(response.content, "windows-1250")
            df = pd.read_csv(StringIO(converted_response), sep=";", low_memory=False)
            name = link.split("/")[-1].replace("export-sluzby-", "").replace(".csv", "-01")
            dfs[name] = df

            # Save if required -> faster further runs
            if save_data:
                write_to_file(save_dir, name + ".csv", converted_response)

        return dfs

    def get_population_data(self, save_data = False):
        """ Downloads file containing data about czech population
            from czso.cz site and returns it as pandas dataframe.
        """

        # Load from saved if required
        save_filename = join(self.save_dir, "population.csv")
        if save_data and exists(save_filename):
            encoding = "utf-8" if platform.system() == "Linux" else "windows-1250"
            return pd.read_csv(save_filename, encoding=encoding)

        # Download data
        response = requests.get(self.url_population, headers = self.request_headers)
        converted_response = str(response.content, "utf-8")
        df = pd.read_csv(StringIO(converted_response))

        # Save data if required
        if save_data:
            write_to_file(self.save_dir, "population.csv", converted_response)

        return df
            