// File: CsvHandler.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Data

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using YSoft.Rqa.AnomalyDetection.Data.Enums;
using YSoft.Rqa.AnomalyDetection.Data.Model.Csv;

namespace YSoft.Rqa.AnomalyDetection.Data.Services
{
    /// <summary>Handles all manipulation with csv files.</summary>
    public class CsvHandler
    {
        private const string DataDirectory = "Data";
        private readonly string _downloadedDataDirectory = $"{DataDirectory}{Path.DirectorySeparatorChar}DownloadedData";
        private readonly string _lastLogFile = $"{DataDirectory}{Path.DirectorySeparatorChar}LastLog.csv";
        private readonly string _referenceDataDirectory = $"{DataDirectory}{Path.DirectorySeparatorChar}ReferenceData";

        /// <summary>Retrieves the timestamp of the last downloaded log.</summary>
        /// <returns>Timestamp of the last downloaded log</returns>
        public DateTime? GetLastDownloadedLogTimestamp() => ReadCsv(_lastLogFile).SingleOrDefault()?.Timestamp;

        /// <summary>Saves the specified timestamp as the last downloaded one.</summary>
        /// <param name="timeStamp">Last downloaded timestamp to save.</param>
        public void SaveLastDownloadedLogTimestamp(DateTime timeStamp) => WriteToCsv(_lastLogFile, new[] {new RequestDataPoint {Timestamp = timeStamp}});

        /// <summary>Writes newly downloaded batch of logs from Graylog to the corresponding csv files.</summary>
        /// <param name="data">Downloaded batch of logs.</param>
        public void WriteDownloadedBatchToCsv(Dictionary<string, Dictionary<string, List<RequestDataPoint>>> data) {
            foreach (var service in data.Keys) {
                foreach (var requestType in data[service].Keys) {
                    var dir = $"{_downloadedDataDirectory}{Path.DirectorySeparatorChar}{service}";
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var file = $"{dir}{Path.DirectorySeparatorChar}{requestType}.csv";
                    WriteToCsv(file, data[service][requestType], File.Exists(file));
                }
            }
        }

        /// <summary>Reads a specified csv file.</summary>
        /// <param name="file">Name of the csv file to be read from.</param>
        /// <returns>List of requests in the csv file.</returns>
        public List<RequestDataPoint> ReadCsv(string file) {
            if (!File.Exists(file))
                return new List<RequestDataPoint>();

            using var reader = new StreamReader(file);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Configuration.RegisterClassMap<RequestDataPointMap>();
            return csv.GetRecords<RequestDataPoint>().ToList();
        }

        /// <summary>Writes a sequence of requests to a csv file.</summary>
        /// <param name="file">Name of the csv file to write into.</param>
        /// <param name="records">Requests to write into the file.</param>
        /// <param name="append">If true, the <paramref name="records" /> are appended to the file, otherwise the file is overwritten.</param>
        public void WriteToCsv(string file, IEnumerable<RequestDataPoint> records, bool append = false) {
            using var writer = append
                ? new StreamWriter(File.Open(file, FileMode.Append))
                : new StreamWriter(file);

            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.Configuration.HasHeaderRecord = !append;
            csv.Configuration.RegisterClassMap<RequestDataPointMap>();
            csv.WriteRecords(records);
        }

        /// <summary>Removes requests from a csv file older then specified interval.</summary>
        /// <param name="file">Name of the file from which to remove the old requests.</param>
        /// <param name="referenceIntervalLength">Number of days from current time from which to remove the data.</param>
        public void RemoveOverdueData(string file, int referenceIntervalLength) {
            var records = ReadCsv(file);
            var oldestLogTime = DateTime.Now.AddDays(-referenceIntervalLength);
            records.RemoveAll(x => x.Timestamp < oldestLogTime);
            WriteToCsv(file, records);
        }

        /// <summary>Deletes all data of specified type.</summary>
        /// <param name="dataType">Type of data to delete.</param>
        /// <param name="serviceName">If specified, only data of a specific service will be deleted.</param>
        public void RemoveExistingData(DataType dataType, string serviceName = null) {
            var toDelete = new List<string>();
            switch (dataType) {
                case DataType.Reference:
                    toDelete.Add(_referenceDataDirectory);
                    break;
                case DataType.Downloaded:
                    toDelete.Add(_downloadedDataDirectory);
                    break;
                case DataType.All:
                    toDelete.Add(_referenceDataDirectory);
                    toDelete.Add(_downloadedDataDirectory);
                    break;
            }

            toDelete = toDelete.Select(dir => $"{dir}{Path.DirectorySeparatorChar}{serviceName}").ToList();
            foreach (var dir in toDelete)
                DeleteDirectory(dir);
        }

        /// <summary>Deletes a directory, if it exists.</summary>
        /// <param name="dir">Name of the directory to delete.</param>
        public void DeleteDirectory(string dir) {
            if (!Directory.Exists(dir))
                return;

            Directory.Delete(dir, true);
        }
    }
}