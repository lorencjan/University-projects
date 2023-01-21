#!/usr/bin/env python3

# File: download.py
# Solution: IZV - project part 1
# Date: 28.10.2020
# Author: Jan Lorenc
# Faculty: Faculty of information technology VUT
# Description: File containing class DataDownloader
#              for downloading and parsing data

import requests
import io
import os
import re
import glob
import csv
import gzip
import numpy as np
import pickle as pkl
from bs4 import BeautifulSoup
from zipfile import ZipFile


class DataDownloader:
    """
    A class responsible for downloading and parsing data about car accidents.

    Attributes
    ----------
    url : str
        Url address from which the class should download the data
    folder : str
        Name of the folder where to download the data and store cache files.
    cache_file : str
        Template for cache file names with format option for a region.
    years : list
        Sorted list of detected years from which are the data
    region_data : dict
        Stores already loaded and parsed data in memory for faster reuse.
    region_files : dict
        Stores data file names for specific regions.
    _data_headers : list
        Stores the column names (= header) for the data.
    _data_col_types : list
        Stores the data type for each column.

    Methods
    -------
    download_data()
        Downloads all .zip files containing data from the site specified
        by url to a local folder specified by folder attribute.
    parse_region_data(region):
        Reads and parses data of specific region,
        downloads data it not yet downloaded.
    get_list(regions = None):
        If the data for the specific region isn't already loaded or cached
        it reads, parses and caches the data for specified regions
        (or all of them), otherwise it gets it from memory/cache files.
    """
    
        
    def __init__(self, url="https://ehw.fit.vutbr.cz/izv/",
                 folder="data", cache_filename="data_{}.pkl.gz"):
        """ Sets source url address and location downloaded and cache data.
            In addition, it initializes constants such as data file names,
            column headers and types. """
        
        self.url = url
        self.folder = folder
        self.cache_file = cache_filename
        self.years = list()
        self.region_data = dict()
        self.region_files = {
            "PHA": "00.csv",
            "STC": "01.csv",
            "JHC": "02.csv",
            "PLK": "03.csv",
            "KVK": "19.csv",
            "ULK": "04.csv",
            "LBK": "18.csv",
            "HKK": "05.csv",
            "PAK": "17.csv",
            "OLK": "14.csv",
            "MSK": "07.csv",
            "JHM": "06.csv",
            "ZLK": "15.csv",
            "VYS": "16.csv",
        }
        
        # Here I deliberately broke the python's line length convention.
        # I thought it better to place 10 on a line for easier navigation
        # (same for the types below).
        self._data_headers = [
            "Id", "Druh pozemní komunikace", "Číslo pozemní komunikace", "Datum", "Den týdne", "Čas", "Druh nehody", "Druh srážky", "Druh pevné překážky", "Charakter nehody",
            "Zavinění nehody", "Přítomen alkohol", "Hlavní příčiny", "Usmrceno", "Těžce zraněno", "Lehce zraněno", "Hmotná škoda", "Druh povrchu vozovky", "Stav povrchu vozovky", "Stav komunikace",
            "Povětrnostní podmínky", "Viditelnost", "Rozhledové poměry", "Dělení komunikace", "Situování nehody na komunikaci", "Řízení provozu v době nehody", "Místní úprava přednosti v jízdě", "Specifická místa a objekty v době nehody", "Směrové poměry", "Počet zúčastněných vozidel",
            "Místo nehody", "Druh křižující komunikace", "Druh vozidla", "Značka vozidla", "Rok výroby", "Charakteristika vozidla", "Smyk", "Vozidlo po nehodě", "Únik provozních/přepravních hmot", "Způsob vyproštění osob",
            "Směr jízdy/postavení vozidla", "Škoda na vozidle", "Kategorie řidiče", "Stav řidiče", "Vnější ovlivnění řidiče", "a", "b", "X souřadnice", "Y souřadnice", "f",
            "g", "h", "i", "j", "k", "l", "n", "o", "p", "q",
            "r", "s", "t", "Lokalita", "Region"
        ]
        
        # There can be missing values -> all integers will be floats for NaN
        # support. The truly float values whose comas need to be replaced by
        # dots are marked as float64 for distinction.
        self._data_col_types = [
            np.float, np.float, np.float, "U10", np.float, np.float32, np.float, np.float, np.float, np.float,
            np.float, np.float, np.float, np.float, np.float, np.float, np.float, np.float, np.float, np.float,
            np.float, np.float, np.float, np.float, np.float, np.float, np.float, np.float, np.float, np.float,
            np.float, np.float, np.float, np.float, np.float, np.float, np.float, np.float, np.float, np.float,
            np.float, np.float, np.float, np.float, np.float, np.float64, np.float64, np.float64, np.float64, np.float64,
            np.float64, "U50", "U50", "U50", "U50", "U50", "U50", "U50", "U50", "U50",
            np.float, np.float, "U50", np.float, "U50"
        ]
            
    def download_data(self):
        """ Downloads all .zip files containing data from the site specified
            by url to a local folder specified by folder attribute. """
        
        # Data should be downloaded only once.
        if(len(glob.glob(self.folder+ "/" + "data*")) > 0):
            return;
        
        # Get zip files' location.
        headers = {
            "Accept-Encoding": "zip, deflate, br",
            "Accept-Language": "cs-CZ",
            "Accept": "application/json",
            "Content-Type": "application/json; charset=UTF-8",
            "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) \
                           AppleWebKit/537.36 (KHTML, like Gecko) \
                           Chrome/85.0.4183.121 \
                           Safari/537.36 OPR/71.0.3770.228",
            "X-Requested-With": "XMLHttpRequest"
        }
        page = requests.get(self.url, headers = headers)
        soup = BeautifulSoup(page.content, "html.parser")
        links = [a["href"]
                 for a
                 in soup.find_all("a", href=re.compile("data.*?\.zip"))]
        
        # Create data folder if it doesn't exist.
        os_path = os.path
        if not os_path.exists(self.folder):
            os.makedirs(self.folder)
        
        # Save zip files.
        # I'd rather download only zips I need (full year/last month)
        # but the task says all -> all it is.
        for link in links:
            response = requests.get(self.url + link, headers = headers)
            zip_name = link.split("/")[1]
            file_path = os_path.join(os.getcwd(), self.folder, zip_name)
            with open(file_path, "wb") as zp:
                zp.write(response.content)
        
    def parse_region_data(self, region):
        """ Reads and parses data of specific region,
            downloads data it not yet downloaded. """
        
        self.download_data()
        
        # We need only the last of a year -> one not containing month or
        # if so, then the last one of them.
        os_path = os.path
        all_files = "";
        if len(self.years) == 0:
            dir_content = str(os.listdir(self.folder))
            self.years = sorted(set(re.findall("[0-9]{4}", dir_content)))
        for year in self.years:
            # Get correct zip file - for the whole year.
            zips = filter(lambda x: re.search("data[^0-9]*?"+year, x),
                          os.listdir(self.folder))
            zip_file = list(zips)
            # If not found, the year hasn't ended -> find last month.
            if len(zip_file) == 0:
                month_zips = filter(lambda x: year in x,
                                    os.listdir(self.folder))
                zip_file = list(month_zips)[-1]
            else:
                zip_file = zip_file[0]
            # Load and decode file.
            file_path = os_path.join(os.getcwd(), self.folder, zip_file)
            with ZipFile(file_path, "r") as zf:
                with zf.open(self.region_files[region], "r") as csv_file:
                    all_files += csv_file.read().decode("cp1250")
        
        # Allocate result arrays -> faster writing to the np array.
        lines = all_files.count(os.linesep)
        values = [np.empty(lines, dtype=self._data_col_types[i])
                  for i
                  in range(len(self._data_col_types))]
        
        # Remove last EOL not to create empty data row.
        if all_files[-2:] == os.linesep:
            all_files = all_files[:-2]
            
        # Load and parse data.
        lines = all_files.split(os.linesep)
        for i, line in enumerate(csv.reader(lines, delimiter=";")):
            for j in range(len(line)):
                if self._data_col_types[j] == np.float:
                    values[j][i] = line[j] if line[j].isdigit() else "NaN"
                elif self._data_col_types[j] == np.float64:
                    values[j][i] = (line[j].replace(",", ".")
                                    if line[j].isdigit()
                                    else "NaN")
                # I want to recompute the time to the amount of minutes,
                # therefore the 32 type for distinction + it is enough.
                elif self._data_col_types[j] == np.float32:
                    values[j][i] = (int(line[j][0:2])*60 + int(line[j][2:4])
                                    if line[j] != "2560"
                                    else "NaN")
                else:
                    values[j][i] = line[j]
        
        values[len(values)-1] = np.full(len(values[0]), region)
        return self._data_headers, values
    
    def get_list(self, regions = None):
        """ If the data for the specific region isn't already loaded or cached
            it reads, parses and caches the data for specified regions
            (or all of them), otherwise it gets it from memory/cache files."""
        
        if regions == None:
            regions = list(self.region_files.keys())
        
        # Load from memory / cache file.
        os_path = os.path
        cached_data = []
        regions_in_memory = self.region_data.keys()
        cache_path = self.folder + "/" + self.cache_file
        for i in range(len(regions)-1, -1, -1):
            if regions[i] in regions_in_memory:
                cached_data.append(self.region_data[regions[i]])
                regions.pop(i)
            elif os_path.exists(cache_path.format(regions[i])):
                file_path = cache_path.format(regions[i])
                with gzip.open(file_path, "rb") as cache_file:
                    loaded_data = pkl.load(cache_file)
                    cached_data.append(loaded_data)
                    self.region_data[regions[i]] = loaded_data
                    regions.pop(i)
        
        # Load and parse.
        regions_data = ([self.parse_region_data(r)[1] for r in regions]
                        if len(regions) > 0
                        else [])
        
        # Cache the new data.
        for region_data in regions_data:
            region = region_data[len(region_data)-1][0]
            with gzip.open(cache_path.format(region), "wb") as cache_file:
                pkl.dump(region_data, cache_file, pkl.HIGHEST_PROTOCOL)
            self.region_data[region] = region_data
        
        # Concatenate region data to the final result.
        regions_data += cached_data
        result = []
        for i in range(len(regions_data[0])):
            result.append(np.concatenate([row[i] for row in regions_data]))
        
        return self._data_headers, result
    
if __name__ == "__main__":
    data_downloader = DataDownloader()
    header, data = data_downloader.get_list(["PHA", "KVK", "JHC"])
    
    regions = ""
    for region in np.unique(data[header.index("Region")]):
        regions += region + ", "
    print("Data jsou stažená pro následující kraje:\n" + regions[:-2])
    
    print("Celkem záznamů:")
    print(data[0].size)
    
    cols = ""
    for col in header:
        cols += col + ", "
    print("Názvy sloupců:\n" + cols[:-2])
