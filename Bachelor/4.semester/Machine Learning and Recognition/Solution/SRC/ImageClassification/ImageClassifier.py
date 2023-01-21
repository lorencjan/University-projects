#!/usr/bin/env python3

import os
import cv2
import numpy as np
import pickle as pkl
from glob import glob
from Misc import Misc

from sklearn.linear_model import LogisticRegression, SGDClassifier
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score
from sklearn.model_selection import train_test_split

class ImageClassifier:
    """Handles all image classification"""
    
    TrainDataTarget = { "Data": [], "Names": [] }
    TrainDataOther = { "Data": [], "Names": [] }
    TestDataTarget = { "Data": [], "Names": [] }
    TestDataOther = { "Data": [], "Names": [] }
    EvalData = { "Data": [], "Names": [] }
    
    PreparedData = dict()
    
    LLR_ModelFile = "ImageClassification/Trained_LogisticRegression.pkl"
    LLR_ResultFile = "ImageClassification/Result_LogisticRegression.txt"
    LLR_Result = dict()
    RF_ModelFile = "ImageClassification/Trained_RandomForest.pkl"
    RF_ResultFile = "ImageClassification/Result_RandomForest.txt"
    RF_Result = dict()
    SGD_ModelFile = "ImageClassification/Trained_SGD.pkl"
    SGD_ResultFile = "ImageClassification/Result_SGD.txt"
    SGD_Result = dict()
    Overall_ResultFile = "ImageClassification/Result_Overall.txt"
        
    @classmethod
    def PrepareDataForTraining(cls):
        """Converts the loaded image data to a training format"""
        X_train = np.array(cls.TrainDataTarget["Data"] + cls.TrainDataOther["Data"])
        y_train = np.r_[np.ones(len(cls.TrainDataTarget["Names"]), dtype=int), np.zeros(len(cls.TrainDataOther["Names"]), dtype=int)]
        X_test = np.array(cls.TestDataTarget["Data"] + cls.TestDataOther["Data"])
        y_test = np.r_[np.ones(len(cls.TestDataTarget["Names"]), dtype=int), np.zeros(len(cls.TestDataOther["Names"]), dtype=int)]
                
        #join the test and train ... it's better to have it shuffled which is what "train_test_split" will do later
        X = np.r_[X_train, X_test]
        cls.PreparedData["X"] = np.array([x.flatten() for x in X])
        cls.PreparedData["y"] = np.r_[y_train, y_test]

    @classmethod
    def TrainModel(cls, model, fileName):
        """Trains a specified model and saves it"""
        #train
        X = cls.PreparedData["X"]
        y = cls.PreparedData["y"]
        x_train, x_test, y_train, y_test = train_test_split(X, y, test_size=0.2)
        model.fit(x_train,y_train)
        #store result so that no need tot rain every time
        with open(fileName, "wb") as file:
            pkl.dump(model, file)
        
        return "{:.2f}".format(accuracy_score(model.predict(x_test), y_test)*100)
    
    @classmethod
    def LoadModel(cls, fileName, trainFunc):
        """Loads a model from specified file, it this doesn't exist, trains it"""
        if not os.path.isfile(fileName):
            trainFunc()
        with open(fileName, "rb") as file:
            return pkl.load(file)
        
    @classmethod
    def PredictProbaResults(cls, model, result, resultFileName):
        """Predicts results using specified model with probability as metrics and saves them"""
        decisions = model.predict(cls.EvalData["Data"])
        probs = model.predict_proba(cls.EvalData["Data"])
        out = ""
        for i in range(len(decisions)):
            score = probs[i][int(decisions[i])]*100
            score = score if decisions[i] == 1 else 100-score
            result[cls.EvalData["Names"][i]] = (score, decisions[i])
            out += cls.EvalData["Names"][i] + " " + "{:.2f}".format(score) + " " + str(decisions[i]) + "\n"
        with open(resultFileName, "w") as file:
            file.write(out)
    
    @classmethod
    def PredictScoreResults(cls, model, result, resultFileName):
        """Predicts results using specified model with the models score as metrics and saves them"""
        decisions = model.predict(cls.EvalData["Data"])
        probs = model.decision_function(cls.EvalData["Data"])
        out = ""
        for i in range(len(decisions)):
            score = "{:.2f}".format(probs[i])
            result[cls.EvalData["Names"][i]] = (score, decisions[i])
            out += cls.EvalData["Names"][i] + " " + score + " " + str(decisions[i]) + "\n"
        with open(resultFileName, "w") as file:
            file.write(out)
        
    @classmethod
    def TrainRandomForest(cls):
        """Trains the random forest classification model"""
        print("    Training random forest model ...")
        score = cls.TrainModel(RandomForestClassifier(), cls.RF_ModelFile)
        print("    Random forest model trained with " + str(score) + "% test data accuracy. It's saved in", cls.RF_ModelFile)
    
    @classmethod
    def PredictByRandomForest(cls):
        """Using sklearn random forest model predicts results"""
        print("  Classifying images using random forest model ...")
        model = cls.LoadModel(cls.RF_ModelFile, cls.TrainRandomForest)
        cls.PredictProbaResults(model, cls.RF_Result, cls.RF_ResultFile)
        print("  Random forest classification completed. It's results can be found in", cls.RF_ResultFile)
            
    @classmethod
    def TrainSGD(cls):
        """Trains the stochastic gradient descent model model and saves it"""
        print("    Training SGD model ...")
        score = cls.TrainModel(SGDClassifier(), cls.SGD_ModelFile)
        print("    SGD model trained with " + str(score) + "% test data accuracy. It's saved in", cls.SGD_ModelFile)
    
    @classmethod
    def PredictBySGD(cls):
        """Using sklearn SGD model predicts results"""
        print("  Classifying images using SGD model ...")
        model = cls.LoadModel(cls.SGD_ModelFile, cls.TrainSGD)
        cls.PredictScoreResults(model, cls.SGD_Result, cls.SGD_ResultFile)
        print("  Stochastic gradient descent classification completed. It's results can be found in", cls.SGD_ResultFile)
    
    @classmethod
    def TrainLinLogisticRegression(cls):
        """Trains the linear logistic regression model and saves it"""
        print("    Training linear logistic regression model ...")
        score = cls.TrainModel(LogisticRegression(solver="liblinear"), cls.LLR_ModelFile)
        print("    Linear logistic regression model trained with " + str(score) + "% test data accuracy. It's saved in", cls.LLR_ModelFile)
    
    @classmethod
    def PredictByLinLogisticRegression(cls):
        """Using sklearn Logistic regression model predicts results"""
        print("  Classifying images using linear logistic model ...")
        model = cls.LoadModel(cls.LLR_ModelFile, cls.TrainLinLogisticRegression)
        cls.PredictProbaResults(model, cls.LLR_Result, cls.LLR_ResultFile)
        print("  Logistic regression classification completed. It's results can be found in", cls.LLR_ResultFile)
                
    @classmethod
    def LoadTrainingData(cls):
        """Loads all .png images from training data directory"""
        cls.TrainDataTarget["Names"], cls.TrainDataTarget["Data"] = cls.LoadPngFiles("ImageClassification/TrainingData/Target")
        cls.TrainDataOther["Names"], cls.TrainDataOther["Data"] = cls.LoadPngFiles("ImageClassification/TrainingData/Other")
        cls.TestDataTarget["Names"], cls.TestDataTarget["Data"] = cls.LoadPngFiles("ImageClassification/TestData/Target")
        cls.TestDataOther["Names"], cls.TestDataOther["Data"] = cls.LoadPngFiles("ImageClassification/TestData/Other")
    
    @classmethod
    def LoadDataToPredict(cls, directory):
        """Loads all .png images from given directory"""
        cls.EvalData["Names"], cls.EvalData["Data"] = cls.LoadPngFiles(directory)
        cls.EvalData["Data"] = np.array(cls.EvalData["Data"]).reshape(len(cls.EvalData["Data"]), -1)
    
    @classmethod
    def LoadPngFiles(cls, dir):
        """Loads all .png images in given directory"""
        data = list()
        names = list()
        for file in glob(dir + '/*.png'):
            data.append(cv2.imread(file))
            names.append(Misc.GetFileName(file, "png"))
        return names, data
    
    @classmethod
    def GetResult(cls):
        """Gets the classifiers final result/decision in decimal percentage as how the single classifiers agree with each other"""
        result = dict()
        out = ""
        for key in cls.LLR_Result:
            decisionSum = cls.LLR_Result[key][1] + cls.RF_Result[key][1] + cls.SGD_Result[key][1]
            percentageScore = float("{:.2f}".format(decisionSum / 3))
            decision = 1 if percentageScore > 0.5 else 0
            result[key] = (percentageScore, decision)
            out += key + " " + str(percentageScore*100) + " " + str(decision) + "\n"
        with open(cls.Overall_ResultFile, "w") as file:
            file.write(out)
        return result
    