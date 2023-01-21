#!/usr/bin/env python3

import sys
from os.path import isdir
from numpy import mean

class Misc:
    """Contains some useful uncategorised functions"""
    
    @staticmethod
    def GetFileName(file, extension):
        """Extracts file base name ... without folder path and extension"""
        name = file.replace("\\", "/")
        arr = name.split("/")
        name = arr[len(arr)-1]
        return name.replace("." + extension, "")
    
    @staticmethod
    def CheckArgs():
        """Checks a returns input from program arguments, first is path to evaluation data, second is optional classiffier selection"""
        argsLen = len(sys.argv)-1 #first is the script name
        if argsLen == 1:
            return (Misc.CheckDir(sys.argv[1]), "both")
        elif argsLen == 2:
            path = Misc.CheckDir(sys.argv[1])
            mode = sys.argv[2]
            if mode != "img" and  mode != "audio" and  mode != "both":
                print("Error: Incorrect classification mode. Enter \"img\"/\"audio\" for only image/audio classification or \"both\" (default).")
                sys.exit(1)
            return (path, mode)
        else:
            print("Error: Incorrect format of input argumnets. First specify a path to the evaluation data, optionally you can add \"img\"/\"audio\" as the second one to choose only one classification.")
            sys.exit(1)
    
    @staticmethod        
    def CheckDir(dirPath):
        """Checks if the path is really a directory ... exits program if not ... program arg checking"""
        if not isdir(dirPath):
            print("Error: Given directory doesn't exist.")
            sys.exit(1)
        return dirPath
