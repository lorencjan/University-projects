#!/usr/bin/env python3

# File: run.py
# Solution: UPA - project
# Date: 7.10.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: Entrypoint (or main() one can say) for the project


from src.data_downloader import DataDownloader
from src.data_convertor import DataConverter
from src.data_checker import DataChecker


if __name__ == "__main__":
    # Download data
    print("Data are downloading ...") 
    downloader = DataDownloader()
    providers = downloader.get_healthcare_providers_data()
    population = downloader.get_population_data()
    print("Data has been downloaded.")    
    
    # Clean and check data
    print("Data are being preprocessed ...") 
    checker = DataChecker()
    providers = checker.check_providers(providers)
    population = checker.check_population(population)
    print("Data has been preprocessed.")    
    
    # Save data to db
    converter = DataConverter("UPA")    
    for name in list(providers):
        print("Storing dataset \'providers_" + name + "\' ...")
        converter.df_to_mongodb(providers[name], "providers_" + name, True)

    print("Storing dataset \'population\' ...")
    converter.df_to_mongodb(population, "population", True)
    print("Data has been stored to db.")
