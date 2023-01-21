# File: data_converter.py
# Solution: UPA - project part 1
# Date: 5.10.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: File containing class DataConverter for storing dataframes to mMngoDb


import pandas as pd
from pymongo import MongoClient
from pymongo.errors import ConnectionFailure


class DataConverter:
  """
  Class responsible for converting data to Nosql database or conversely
  """
  
  def __init__(self,db_name : str) -> None:
    """
    Connects to the mongo database, if it can't connect the program is exiting with exit code 1
    """
    
    self.db = MongoClient('mongodb://root:password@localhost:27017')
    
    # Connection test
    try:
      self.db.admin.command('ping')
    except ConnectionFailure:
      print("Server not available")
      exit(1)

    self.database = self.db[db_name]
    

  def df_to_mongodb(self, df : pd.DataFrame, collection_name : str, drop : bool = False) -> None:
    """
    Converts pandas dataframe to mongoDB. 
    With the parameter "drop" the existing collection is deleted. The default setting is False 
    """

    if drop:
      self.mongodb_drop(collection_name)

    db_collection = self.database[collection_name]
    df.reset_index(inplace=True)
    db_collection.insert_many(df.to_dict("records"))
    

  def mongodb_to_df(self,collection_name : str, query = {}, no_index = True, aggr = []) -> pd.DataFrame:
    """
    Converts mongoDB collection to pandas dataframe. 
    """
    
    db = self.database[collection_name]
    cursor = db.aggregate(aggr) if aggr else db.find(query)
    df =  pd.DataFrame(list(cursor))
    
    if no_index and not query and not aggr:
      del df['_id']
    
    return df


  def get_collection_names(self) -> list:
    """
    Returns all collection names
    """

    return self.database.collection_names(include_system_collections=False)



  def mongodb_drop(self,collection_name : str) -> None:
    """
    Drops collection
    """

    self.database[collection_name].drop()
