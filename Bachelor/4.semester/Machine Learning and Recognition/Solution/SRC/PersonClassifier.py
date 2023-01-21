#!/usr/bin/env python3

import sys
from ImageClassification.ImageClassifier import ImageClassifier
from AudioClassification.AudioClassifier import AudioClassifier
from Misc import Misc

def main():
    evalDataPath, mode = Misc.CheckArgs()
    print("Loading data ...")
    if mode != "img":
        AudioClassifier.LoadTrainingData()
        AudioClassifier.LoadDataToPredict(evalDataPath)
    if mode != "audio":
        ImageClassifier.LoadTrainingData()
        ImageClassifier.LoadDataToPredict(evalDataPath)
    print("Data loaded")
    
    if mode != "img":
        print("Audio classification ...")
        AudioClassifier.PredictByGMM()
        print("Audio samples classified")
    
    if mode != "audio":
        print("Image classification ...")
        ImageClassifier.PrepareDataForTraining()
        ImageClassifier.PredictByLinLogisticRegression()
        ImageClassifier.PredictBySGD()
        ImageClassifier.PredictByRandomForest()
        print("Images classified")
    
    print("Evaluating results ...")
    if mode == "audio":
        print("Classification result of audio GMM is stored in " + AudioClassifier.GMM_ResultFile)
        sys.exit(0)
    if mode == "img":
        ImageClassifier.GetResult()
        print("Overall image classification result is stored in " + ImageClassifier.Overall_ResultFile)
        print("Individual classification results are in the same folder with adequate names")
        sys.exit(0)
    
    audioResult = AudioClassifier.GetResult()
    imgResult = ImageClassifier.GetResult()
    outputFile = "classification_result.txt"
    out = ""
    for key in audioResult:                     #the decision tree is explained in documentation
        if audioResult[key][1] == 1:            #here for short the score is percentage of decision agreement of all 4 classification methods
            if imgResult[key][0] == 0:          #50:50 is inclined to positive decision
                decision = 0
                score = 25
            else:
                decision = 1
                if imgResult[key][0] == 0.33:
                    score = 50
                else:
                    score = 100 if imgResult[key][0] == 1 else 75
        else:
            if imgResult[key][0] == 1:
                decision = 1
                score = 75
            elif imgResult[key][0] == 0.67:
                decision = 1
                score = 50
            else:
                decision = 0
                score = 0 if imgResult[key][0] == 0 else 25
        out += key + " " + str(score) + " " + str(decision) + "\n"
    with open(outputFile, "w") as file:
        file.write(out)
    print("Classification results stored in " + outputFile)
    print("To see individual classification methods' result see ImageClassification and AudioClassification folders")
        
    
    
    
if __name__ == "__main__":
    main()