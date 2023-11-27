// File: ErrorAnomalyDetector.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Application

using System.Linq;
using Microsoft.Data.Analysis;
using YSoft.Rqa.AnomalyDetection.Application.Model;

namespace YSoft.Rqa.AnomalyDetection.Application.Services
{
    /// <summary>Provides operations for anomaly detection in request errors.</summary>
    public class ErrorAnomalyDetector
    {
        /// <summary>Compares the error counts in input and reference data and decides whether there is an error anomaly.</summary>
        /// <param name="inputData"></param>
        /// <param name="referenceData"></param>
        /// <param name="anomalyLimit">Percentage limit over which the difference will prove anomalous.</param>
        /// <returns></returns>
        public bool IsInputAnomalous(DataFrame inputData, DataFrame referenceData, double anomalyLimit = 0.1) {
            var inputErrors = inputData.GroupBy(Constants.Errors).Count().OrderBy(Constants.Errors);
            var referenceErrors = referenceData.GroupBy(Constants.Errors).Count().OrderBy(Constants.Errors);

            //there cannot be any new error counts
            var inputErrorClasses = inputErrors.Get<int>(Constants.Errors);
            var referenceErrorClasses = referenceErrors.Get<int>(Constants.Errors);
            if (inputErrorClasses.Except(referenceErrorClasses).Any())
                return true;

            //there can be missing ones, however they need to be in anomaly limit -> add the missing ones with 0 count
            var missingClasses = referenceErrorClasses.Except(inputErrorClasses).ToList();
            if (missingClasses.Any()) {
                var df = referenceErrors.Head(missingClasses.Count);
                df.Set(new PrimitiveDataFrameColumn<int>(Constants.Errors, missingClasses));
                df.Set(new PrimitiveDataFrameColumn<int>(Constants.Timestamp, Enumerable.Repeat(0, missingClasses.Count)));
                inputErrors = inputErrors.Append(df.Rows).OrderBy(Constants.Errors);
            }
            
            var inputErrorCounts = inputErrors.Get<long>(Constants.Timestamp);
            var referenceErrorCounts = referenceErrors.Get<long>(Constants.Timestamp);

            var inputErrorPercentages = inputErrorCounts.Divide<double>(inputData.Length());
            var referenceErrorPercentages = referenceErrorCounts.Divide<double>(referenceData.Length());

            var differences = inputErrorPercentages.Subtract(referenceErrorPercentages).Abs();
            return differences.ElementwiseGreaterThan(anomalyLimit).Any();
        }
    }
}