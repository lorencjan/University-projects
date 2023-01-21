#!/usr/bin/env python3

import os
import numpy as np
import pickle as pkl
from glob import glob
from scipy.io import wavfile
from ikrlib import mfcc, train_gmm, logpdf_gmm
from Misc import Misc

class AudioClassifier:
    """Handles all audio classification"""
    
    TrainDataTarget = { "Data": [], "Names": [] }
    TrainDataOther = { "Data": [], "Names": [] }
    TestDataTarget = { "Data": [], "Names": [] }
    TestDataOther = { "Data": [], "Names": [] }
    EvalData = { "Data": [], "Names": [] }
    
    GMM_ModelFile = "AudioClassification/Trained_GMM.pkl"
    GMM_ResultFile = "AudioClassification/Result_GMM.txt"
    GMM_Result = dict()
                     
    @classmethod
    def TrainGmm(cls, numOfIterations=60):
        """Trains complete GMM model based on training data"""
        print("    Training GMM model ...")
        x_target = np.vstack(cls.TrainDataTarget["Data"])
        x_other = np.vstack(cls.TrainDataOther["Data"])
        
        N_gmm_components_target = 6
        N_gmm_components_other = 6
        #mean vectors ... pick the first one randomly
        Mu_target = x_target[np.random.randint(1, len(x_target), N_gmm_components_target)]
        Mu_other = x_other[np.random.randint(1, len(x_other), N_gmm_components_other)]
        #covariance matrices
        Covs_target = [np.var(x_target, axis=0)] * N_gmm_components_target
        Covs_other = [np.var(x_other, axis=0)] * N_gmm_components_other
        #initial weights ... evently distributed
        Ws_target = np.ones(N_gmm_components_target) / N_gmm_components_target
        Ws_other = np.ones(N_gmm_components_other) / N_gmm_components_other

        #training
        for i in range(numOfIterations):
            [Ws_target, Mu_target, Covs_target, TTL_target] = train_gmm(x_target, Ws_target, Mu_target, Covs_target)
            [Ws_other, Mu_other, Covs_other, TTL_other] = train_gmm(x_other, Ws_other, Mu_other, Covs_other)
        
        #store result so that no need tot rain every time
        with open(cls.GMM_ModelFile, "wb") as file:
            pkl.dump([Ws_target, Mu_target, Covs_target, Ws_other, Mu_other, Covs_other], file)
        print("    GMM trained successfuly. It's parameters are stored in", cls.GMM_ModelFile)
        
    @classmethod
    def PredictByGMM(cls):
        """Get's the trained GMM model from a file or train it if it doesn't exist and compute score for each data point"""
        print("  Classifying audio using GMM ...")        
        
        #loading model parameters
        if not os.path.isfile(cls.GMM_ModelFile):
            cls.TrainGmm()
        with open(cls.GMM_ModelFile, "rb") as file:
            Ws_target, Mu_target, Covs_target, Ws_other, Mu_other, Covs_other = pkl.load(file)
                    
        #predicting
        out = ""
        for i in range(len(cls.EvalData["Data"])):
            ll_target = logpdf_gmm(cls.EvalData["Data"][i], Ws_target, Mu_target, Covs_target)
            ll_other = logpdf_gmm(cls.EvalData["Data"][i], Ws_other, Mu_other, Covs_other)
            score = "{:.2f}".format(sum(ll_target) - sum(ll_other)) #a-priori probrabilities are the same ... can be omitted
            decision = 1 if float(score) > 0 else 0
            cls.GMM_Result[cls.EvalData["Names"][i]] = (score, decision)
            out += cls.EvalData["Names"][i] + " " + score + " " + str(decision) + "\n"
        
        #saving results
        with open(cls.GMM_ResultFile, "w") as file:
            file.write(out)    
        print("  Audio GMM classification completed. It's results can be found in", cls.GMM_ResultFile)
                        
    @classmethod
    def LoadTrainingData(cls):
        """Loads all .wav files from training data directory"""
        cls.TrainDataTarget["Names"], cls.TrainDataTarget["Data"] = cls.LoadWavFiles("AudioClassification/TrainingData/Target")
        cls.TrainDataOther["Names"], cls.TrainDataOther["Data"] = cls.LoadWavFiles("AudioClassification/TrainingData/Other")
        cls.TestDataTarget["Names"], cls.TestDataTarget["Data"] = cls.LoadWavFiles("AudioClassification/TestData/Target")
        cls.TestDataOther["Names"], cls.TestDataOther["Data"] = cls.LoadWavFiles("AudioClassification/TestData/Other")
    
    @classmethod
    def LoadDataToPredict(cls, directory):
        """Loads all .wav test files from given directory"""
        cls.EvalData["Names"], cls.EvalData["Data"] = cls.LoadWavFiles(directory)
    
    @classmethod
    def LoadWavFiles(cls, dir):
        """Loads all .wav files in given directory"""
        dataset = list()
        names = list()
        for file in glob(dir + '/*.wav'):
            sampleRate, data = wavfile.read(file)
            dataset.append(mfcc(data, 400, 240, 512, sampleRate, 23, 13))
            names.append(Misc.GetFileName(file, "wav"))
        return names, dataset    

    @classmethod
    def GetResult(cls):
        """Gets the classifiers final result/decision"""
        return cls.GMM_Result
    