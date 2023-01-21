# File: data_checker.py
# Solution: UPA - project part 1
# Date: 27.10.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: File containing class DataChecker for transforming and checking data


import numpy as np
import pandas as pd
from numpy.core.numeric import NaN
from pandas._libs.tslibs.nattype import NaT


class DataChecker():
  """
  Class responsible for transforming and checking data
  """

  def __init__(self) -> None:

    # Types for population dataset
    self.population_types = {
        "idhod": "int64", "hodnota": "int64", "stapro_kod": "int64", "pohlavi_cis": "int64", "pohlavi_kod": "int64",
        "vek_cis": "int64", "vek_kod": "int64", "vuzemi_cis": "int64", "vuzemi_kod": "int64", "casref_do": "datetime64",
        "pohlavi_txt": "str", "vek_txt": "str", "vuzemi_txt": "str"
    }

    # Header for population dataset
    self.population_header = {
        0: "idhod", 1: "hodnota", 2: "stapro_kod", 3: "pohlavi_cis", 4: "pohlavi_kod", 5: "vek_cis", 6: "vek_kod",
        7: "vuzemi_cis", 8: "vuzemi_kod", 9: "casref_do", 10: "pohlavi_txt", 11: "vek_txt", 12: "vuzemi_txt"
    }

    # Age range in code and txt
    self.population_age = {
        -1:-1,
        400000600005000: "0 až 5 (více nebo rovno 0 a méně než 5)",
        400005610010000: "5 až 10 (více nebo rovno 5 a méně než 10)", 
        410010610015000: "10 až 15 (více nebo rovno 10 a méně než 15)",
        410015610020000: "15 až 20 (více nebo rovno 15 a méně než 20)",
        410020610025000: "20 až 25 (více nebo rovno 20 a méně než 25)",
        410025610030000: "25 až 30 (více nebo rovno 25 a méně než 30)",
        410030610035000: "30 až 35 (více nebo rovno 30 a méně než 35)",
        410035610040000: "35 až 40 (více nebo rovno 35 a méně než 40)",
        410040610045000: "40 až 45 (více nebo rovno 40 a méně než 45)",
        410045610050000: "45 až 50 (více nebo rovno 45 a méně než 50)",
        410050610055000: "50 až 55 (více nebo rovno 50 a méně než 55)",
        410055610060000: "55 až 60 (více nebo rovno 55 a méně než 60)",
        410060610065000: "60 až 65 (více nebo rovno 60 a méně než 65)",
        410065610070000: "65 až 70 (více nebo rovno 65 a méně než 70)",
        410070610075000: "70 až 75 (více nebo rovno 70 a méně než 75)",
        410075610080000: "75 až 80 (více nebo rovno 75 a méně než 80)",
        410080610085000: "80 až 85 (více nebo rovno 80 a méně než 85)",
        410085610090000: "85 až 90 (více nebo rovno 85 a méně než 90)",
        410090610095000: "90 až 95 (více nebo rovno 90 a méně než 95)",
        410095799999000: "Od 95 (více nebo rovno 95)"
    }
    
    # Gender with number code and txt
    self.population_gender = {-1:-1, 1: "muž", 2: "žena"}

    # Providers data types for long version of dataset
    self.provider_types_long = {
        "ZdravotnickeZarizeniId": "int64",
        "PCZ": "int64",
        "PCDP": "int64",
        "NazevCely": "str",
        "ZdravotnickeZarizeniKod": "int64", 
        "DruhZarizeniKod": "int64",
        "DruhZarizeni": "str",
        "DruhZarizeniSekundarni": "str",
        "Obec": "str",
        "Psc": "int64",
        "Ulice": "str",
        "CisloDomovniOrientacni": "str",
        "Kraj": "str",
        "KrajKod": "str",
        "Okres": "str",
        "OkresKod": "str",
        "SpravniObvod": "str",
        "PoskytovatelTelefon": "str",
        "PoskytovatelFax": "str",
        "DatumZahajeniCinnosti": "datetime64",
        "IdentifikatorDatoveSchranky": "str",
        "PoskytovatelEmail": "str",
        "PoskytovatelWeb": "str",
        "DruhPoskytovatele": "str",                
        "PoskytovatelNazev": "str",
        "Ico": "int64",
        "TypOsoby": "str",
        "PravniFormaKod": "str",
        "KrajKodSidlo": "str",
        "KrajSidlo": "str",
        "OkresKodSidlo": "str",
        "OkresSidlo": "str",                    
        "PscSidlo": "int64",                     
        "ObecSidlo": "str",                       
        "UliceSidlo": "str",                      
        "CisloDomovniOrientacniSidlo": "str",     
        "OborPece": "str",                        
        "FormaPece": "str",                       
        "DruhPece": "str",                    
        "OdbornyZastupce": "str",                 
        "GPS": "str",                             
        "LastModified": "datetime64"
    }

    # Providers data types for short version of dataset
    self.provider_types_short = {
        "ZdravotnickeZarizeniId": "int64",
        "PCZ": "int64",
        "PCDP": "int64",
        "NazevCely": "str",
        "DruhZarizeni": "str",     
        "Obec": "str",
        "Psc": "int64",
        "Ulice": "str",
        "CisloDomovniOrientacni": "str",
        "Kraj": "str",
        "KrajKod": "str",
        "Okres": "str",
        "OkresKod": "str",
        "SpravniObvod": "str",
        "PoskytovatelTelefon": "str",
        "PoskytovatelFax": "str",
        "DatumZahajeniCinnosti": "datetime64",
        "IdentifikatorDatoveSchranky": "str",
        "PoskytovatelEmail": "str",
        "PoskytovatelWeb": "str",
        "PoskytovatelNazev": "str",
        "Ico": "int64",
        "TypOsoby": "str",
        "PravniFormaKod": "str",
        "KrajKodSidlo": "str",
        "KrajSidlo": "str",
        "OkresKodSidlo": "str",
        "OkresSidlo": "str",                    
        "PscSidlo": "int64",                     
        "ObecSidlo": "str",                       
        "UliceSidlo": "str",                      
        "CisloDomovniOrientacniSidlo": "str",     
        "OborPece": "str",                        
        "FormaPece": "str",                       
        "DruhPece": "str",                    
        "OdbornyZastupce": "str",                 
        "GPS": "str",                             
        "LastModified": "datetime64"
    }

    # Header for older dataset from providers until 1.4.2020
    self.provider_header_short = {
        0: "ZdravotnickeZarizeniId",
        1: "PCZ",
        2: "PCDP",
        3: "NazevCely",
        4: "DruhZarizeni",
        5: "Obec",
        6: "Psc",
        7: "Ulice",
        8: "CisloDomovniOrientacni",
        9: "Kraj",
        10: "KrajKod",
        11: "Okres",
        12: "OkresKod",
        13: "SpravniObvod",
        14: "PoskytovatelTelefon",
        15: "PoskytovatelFax",
        16: "DatumZahajeniCinnosti",
        17: "IdentifikatorDatoveSchranky",
        18: "PoskytovatelEmail",
        19: "PoskytovatelWeb",
        20: "PoskytovatelNazev",
        21: "Ico",
        22: "TypOsoby",
        23: "PravniFormaKod",
        24: "KrajKodSidlo",
        25: "KrajSidlo",
        26: "OkresKodSidlo",
        27: "OkresSidlo",
        28: "PscSidlo",
        29: "ObecSidlo",
        30: "UliceSidlo",
        31: "CisloDomovniOrientacniSidlo",
        32: "OborPece",
        33: "FormaPece",
        34: "DruhPece",
        35: "OdbornyZastupce",
        36: "GPS",
        37: "LastModified",
    }

    # Headers for dataset from providers since 1.7.2020 (in between is just 1 field missing -> handled below)
    self.provider_header_long = {
        0: "ZdravotnickeZarizeniId",
        1: "PCZ",
        2: "PCDP",
        3: "NazevCely",
        4: "ZdravotnickeZarizeniKod",
        5: "DruhZarizeniKod",
        6: "DruhZarizeni",
        7: "DruhZarizeniSekundarni",
        8: "Obec",
        9: "Psc",
        10: "Ulice",
        11: "CisloDomovniOrientacni",
        12: "Kraj",
        13: "KrajKod",
        14: "Okres",
        15: "OkresKod",
        16: "SpravniObvod",
        17: "PoskytovatelTelefon",
        18: "PoskytovatelFax",
        19: "DatumZahajeniCinnosti",
        20: "IdentifikatorDatoveSchranky",
        21: "PoskytovatelEmail",
        22: "PoskytovatelWeb",
        23: "DruhPoskytovatele",
        24: "PoskytovatelNazev",
        25: "Ico",
        26: "TypOsoby",
        27: "PravniFormaKod",
        28: "KrajKodSidlo",
        29: "KrajSidlo",
        30: "OkresKodSidlo",
        31: "OkresSidlo",
        32: "PscSidlo",
        33: "ObecSidlo",
        34: "UliceSidlo",
        35: "CisloDomovniOrientacniSidlo",
        36: "OborPece",
        37: "FormaPece",
        38: "DruhPece",
        39: "OdbornyZastupce",
        40: "GPS",
        41: "LastModified",
    }

    # Code and district name
    self.district_code_name = {
        "CZ0100": "Praha",
        "CZ0201": "Benešov",
        "CZ0202": "Beroun",
        "CZ0203": "Kladno",
        "CZ0204": "Kolín",
        "CZ0205": "Kutná Hora",
        "CZ0206": "Mělník",
        "CZ0207": "Mladá Boleslav",
        "CZ0208": "Nymburk",
        "CZ0209": "Praha-východ",
        "CZ020A": "Praha-západ",
        "CZ020B": "Příbram",
        "CZ020C": "Rakovník",
        "CZ0311": "České Budějovice",
        "CZ0312": "Český Krumlov",
        "CZ0313": "Jindřichův Hradec",
        "CZ0314": "Písek",
        "CZ0315": "Prachatice",
        "CZ0316": "Strakonice",
        "CZ0317": "Tábor",
        "CZ0321": "Domažlice",
        "CZ0322": "Klatovy",
        "CZ0323": "Plzeň-město",
        "CZ0324": "Plzeň-jih",
        "CZ0325": "Plzeň-sever",
        "CZ0326": "Rokycany",
        "CZ0327": "Tachov",
        "CZ0411": "Cheb",
        "CZ0412": "Karlovy Vary",
        "CZ0413": "Sokolov",
        "CZ0421": "Děčín",
        "CZ0422": "Chomutov",
        "CZ0423": "Litoměřice",
        "CZ0424": "Louny",
        "CZ0425": "Most",
        "CZ0426": "Teplice",
        "CZ0427": "Ústí nad Labem",
        "CZ0511": "Česká Lípa",
        "CZ0512": "Jablonec nad Nisou",
        "CZ0513": "Liberec",
        "CZ0514": "Semily",
        "CZ0521": "Hradec Králové",
        "CZ0522": "Jičín",
        "CZ0523": "Náchod",
        "CZ0524": "Rychnov nad Kněžnou",
        "CZ0525": "Trutnov",
        "CZ0531": "Chrudim",
        "CZ0532": "Pardubice",
        "CZ0533": "Svitavy",
        "CZ0534": "Ústí nad Orlicí",
        "CZ0631": "Havlíčkův Brod",
        "CZ0632": "Jihlava",
        "CZ0633": "Pelhřimov",
        "CZ0634": "Třebíč",
        "CZ0635": "Žďár nad Sázavou",
        "CZ0641": "Blansko",
        "CZ0642": "Brno-město",
        "CZ0643": "Brno-venkov",
        "CZ0644": "Břeclav",
        "CZ0645": "Hodonín",
        "CZ0646": "Vyškov",
        "CZ0647": "Znojmo",
        "CZ0711": "Jeseník",
        "CZ0712": "Olomouc",
        "CZ0713": "Prostějov",
        "CZ0714": "Přerov",
        "CZ0715": "Šumperk",
        "CZ0721": "Kroměříž",
        "CZ0722": "Uherské Hradiště",
        "CZ0723": "Vsetín",
        "CZ0724": "Zlín",
        "CZ0801": "Bruntál",
        "CZ0802": "Frýdek-Místek",
        "CZ0803": "Karviná",
        "CZ0804": "Nový Jičín",
        "CZ0805": "Opava",
        "CZ0806": "Ostrava-město",
        "CZZZZZ": "Extra-Regio",
    }

    # Code and region name
    self.region_code_name = {
        "CZ010": "Hlavní město Praha",
        "CZ020": "Středočeský kraj",
        "CZ031": "Jihočeský kraj",
        "CZ032": "Plzeňský kraj",
        "CZ041": "Karlovarský kraj",
        "CZ042": "Ústecký kraj",
        "CZ051": "Liberecký kraj",
        "CZ052": "Královéhradecký kraj",
        "CZ053": "Pardubický kraj",
        "CZ063": "Kraj Vysočina",
        "CZ064": "Jihomoravský kraj",
        "CZ071": "Olomoucký kraj",
        "CZ072": "Zlínský kraj",
        "CZ080": "Moravskoslezský kraj",
        "CZZZZ": "Extra-Regio",
    }


  def check_population(self, df : pd.DataFrame) -> pd.DataFrame:
    """ 
    Replaces NaN and NaT values with -1.
    Retypes values of columns.
    Checks other values and returns pd.DataFrame
    """

    # Replace NaN with -1
    df = df.replace(NaN, -1)
    df = df.replace(NaT, -1)
    # Convert pd.DataFrame to numpy_array - it's faster
    np_arr = df.to_numpy()

    for row in np_arr:
      # idhod 
      if row[0] < -1:
        row[0] = -2
      # hodnota
      if row[1] < 0:
        row[1] = -2
      # stapro_kod - 2406 permanent residence
      if not row[2] in [-1,2406]:
        row[2] = -2
      # check gender(pohlavi_cis, pohlavi_kod, pohlavi_txt)
      if row[3] in [-1,102] and row[4] in self.population_gender.keys(): 
        if not row[10] == self.population_gender[int(row[4])]:
          row[10] = -2
      else:  
        row[4] = -2
        row[3] = -2
        row[10] = -2        
      # vek_cis
      if not row[5] in [-1,7700]:
        row[5] = -2      
      # testing age number and age txt
      if row[6] in self.population_age.keys(): 
        if not row[11] == self.population_age[int(row[6])]:
          row[11] = -2
      else:
        row[6] = -2
        row[11] = -2      
      # vuzemi_cis
      if not row[7] in [97,100,101]:
        row[7] = -2

    df = pd.DataFrame(np_arr)  # Convert numpy_array to pd.DataFrame
    df = df.rename(self.population_header ,axis='columns')  # Rename columns
    df = df.astype(self.population_types)  # Retype

    return df

  def check_providers(self, df : pd.DataFrame) -> pd.DataFrame:
    """
    Replaces NaN and NaT values with -1.
    Retypes values of columns.
    Checks other values and returns pd.DataFrame
    """

    for record in df:
      # Replace NaN and NaT with -1
      df[record] = df[record].replace(NaN, -1)
      df[record] = df[record].replace(NaT, -1)
      # Convert to numpy array - it's faster
      np_arr = df[record].to_numpy()
      
      for row in np_arr:
        if row[1] > 0:
          row[1] = -2
        if row[2] < 0: 
          row[2] = -2
        # Older datasets have only 38 columns
        if row.shape == (42,):
          # Check value of TypOsoby
          if not row[26] in [-1, "fyzická osoba", "právnická osoba"]:
            row[26] = -2
          # Check region code and region name
          if row[13] in self.region_code_name.keys():
            if not row[12] == self.region_code_name[row[13]]:
              row[12] = -2
          else:
            row[12] = -2
            row[13] = -2
          # Check district code and region name 
          if row[15] in self.district_code_name.keys():
            if not row[14] == self.district_code_name[row[15]]:
              row[14] = -2
          else:
            row[14] = -2
            row[15] = -2
          # Check region code and region name for residence
          if row[28] in self.region_code_name.keys():
            if not row[29] == self.region_code_name[row[28]]:
              row[28] = -2
          else:
              row[28] = -2
              row[29] = -2
          # Check district code and region name for residence
          if row[30] in self.district_code_name.keys():
            if not row[31] == self.district_code_name[row[30]]:
              row[31] = -2
          else:
            row[30] = -2
            row[31] = -2
        # Older datasets
        else:
          # Check value of TypOsoby
          if not row[22] in [-1, "fyzická osoba", "právnická osoba"]:
            row[22] = -2
          # Check region code and region name
          if row[10] in self.region_code_name.keys():
            if not row[9] == self.region_code_name[row[10]]:
              row[10] = -2
          else:
            row[10] = -2
            row[9] = -2
          # Check district code and region name 
          if row[12] in self.district_code_name.keys():
            if not row[11] == self.district_code_name[row[12]]:
              row[11] = -2
          else:
            row[11] = -2
            row[12] = -2
          # Check region code and region name for residence
          if row[24] in self.region_code_name.keys():
            if not row[25] == self.region_code_name[row[24]]:
              row[24] = -2
          else:
            row[24] = -2
            row[25] = -2
          # Check district code and region name for residence
          if row[26] in self.district_code_name.keys():
            if not row[27] == self.district_code_name[row[26]]:
              row[27] = -2
          else:
            row[27] = -2
            row[26] = -2

      # Convert numpy_array to pd.DataFrame
      df[record] = pd.DataFrame(np_arr)
      # Rename and retype columns
      if np_arr[0].shape == (42,):   # 2020-07 - 2021-10
        df[record] = df[record].rename(self.provider_header_long, axis='columns')
        df[record] = df[record].astype(self.provider_types_long)
      elif np_arr[0].shape == (41,): # 2020-04, 2020-05, 2020-06 ... same as header_long, just without DruhPoskytovatele 
        custom_header = dict(self.provider_header_long)
        custom_types = dict(self.provider_types_long)
        custom_header.pop(23)
        custom_types.pop("DruhPoskytovatele")
        custom_header = {key: val for key, val in enumerate(custom_header.values())}
        df[record] = df[record].rename(custom_header, axis='columns')
        df[record] = df[record].astype(custom_types)
      elif np_arr[0].shape == (45,): # after 2021-10 ... new 3 fields
        custom_header = dict(self.provider_header_long)
        custom_types = dict(self.provider_types_long)
        custom_header[39] = "RozsahPece"
        custom_header[40] = "PocetLuzek"
        custom_header[41] = "PoznamkaKeSluzbe"
        custom_header[42] = "OdbornyZastupce"
        custom_header[43] = "GPS"
        custom_header[44] = "LastModified"
        custom_types["RozsahPece"] = "str"
        custom_types["PocetLuzek"] = "int"
        custom_types["PoznamkaKeSluzbe"] = "str"
        df[record] = df[record].rename(custom_header, axis='columns')
        df[record] = df[record].astype(custom_types)
      else: # before 2020-04
        df[record] = df[record].rename(self.provider_header_short, axis='columns')
        df[record] = df[record].astype(self.provider_types_short)
      
    return df
    