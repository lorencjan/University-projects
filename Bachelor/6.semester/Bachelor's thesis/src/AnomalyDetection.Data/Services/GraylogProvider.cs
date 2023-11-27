// File: GraylogProvider.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Data

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using YSoft.Rqa.AnomalyDetection.Data.Model.Csv;
using YSoft.Rqa.AnomalyDetection.Data.Model.Graylog;

namespace YSoft.Rqa.AnomalyDetection.Data.Services
{
    /// <summary>Handles downloading data from Graylog.</summary>
    public class GraylogProvider
    {
        private const string UrlColon = "%3A";
        private const string UrlSpace = "%20";
        private const string UrlEmptyString = "%02%03";
        private const uint Limit = 10_000;
        private const uint DownloadWindow = 10;

        public double UtcHoursDiff => (DateTime.Now - DateTime.UtcNow).TotalHours;

        private readonly string _baseUrl;
        private readonly HttpClient _client;
        private readonly IOptions<GraylogConfiguration> _config;
        private readonly CsvHandler _csvHandler;
        private readonly Dictionary<string, Dictionary<string, List<DateTime>>> _errorLogs;
        private readonly ILogger<GraylogProvider> _logger;

        public GraylogProvider(ILogger<GraylogProvider> logger, IOptions<GraylogConfiguration> config, CsvHandler csvHandler) {
            var clientHandler = new HttpClientHandler {ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true};
            _client = new HttpClient(clientHandler);
            _config = config;
            var auth = Encoding.ASCII.GetBytes($"{_config.Value.Login}:{_config.Value.Password}");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(auth));
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _logger = logger;
            _csvHandler = csvHandler;
            _errorLogs = new Dictionary<string, Dictionary<string, List<DateTime>>>();
            _baseUrl = $"https://{_config.Value.Host}/api/search/universal/absolute";
        }

        /// <summary>Performs the batch log downloading algorithm.</summary>
        /// <param name="dateTimeInterval">Interval in which to download data.</param>
        /// <param name="serviceName">If specified, only logs from this service will be downloaded.</param>
        /// <returns></returns>
        public async Task DownloadLogs(DateTimeInterval dateTimeInterval, string serviceName = null) {
            do {
                await DownloadErrorLogs(dateTimeInterval, serviceName);
                await DownloadRequestLogs(dateTimeInterval, serviceName);
                dateTimeInterval = new DateTimeInterval(dateTimeInterval.End, DateTime.UtcNow);
            }
            while (dateTimeInterval.Length.TotalMinutes > DownloadWindow);
        }

        /// <summary>Downloads and deserializes all error logs.</summary>
        /// <param name="dateTimeInterval">Interval in which to download data.</param>
        /// <param name="serviceName">If specified, only logs from this service will be downloaded.</param>
        /// <returns></returns>
        private async Task DownloadErrorLogs(DateTimeInterval dateTimeInterval, string serviceName = null) {
            var fields = new List<string> {"source", "timestamp", "requestHandler"};
            var fieldFilters = CreateFieldFilters(_config.Value.ErrorLogsStreamId, serviceName);
            var url = BuildRequestQueryWithVariableDateInterval(fields, fieldFilters);

            var limitedInterval = dateTimeInterval.LimitedInterval();
            List<ErrorLog> logs;
            do {
                var finalUrl = string.Format(url, GraylogUrlDate(limitedInterval.Start), GraylogUrlDate(limitedInterval.End));
                var jsonLogs = await DownloadLogMessages(finalUrl);
                logs = ParseLogs<ErrorLog>(jsonLogs);
                GroupErrorLogs(logs);
                limitedInterval.MoveLimitWindow(logs.LastOrDefault(), dateTimeInterval.End);
            }
            while (logs.Count == Limit || limitedInterval.End != dateTimeInterval.End);
        }

        /// <summary>
        /// Downloads and deserializes all request logs.
        /// It than counts errors for each request and saves the requests to csv files.
        /// </summary>
        /// <param name="dateTimeInterval">Interval in which to download data.</param>
        /// <param name="serviceName">If specified, only logs from this service will be downloaded.</param>
        /// <returns></returns>
        private async Task DownloadRequestLogs(DateTimeInterval dateTimeInterval, string serviceName = null) {
            var fields = new List<string> {"source", "timestamp", "requestHandler", "duration"};
            var fieldFilters = CreateFieldFilters(_config.Value.RequestLogsStreamId, serviceName);
            var url = BuildRequestQueryWithVariableDateInterval(fields, fieldFilters);

            var limitedInterval = dateTimeInterval.LimitedInterval();
            List<RequestLog> logs;
            RequestLog lastLog = null;
            do {
                var finalUrl = string.Format(url, GraylogUrlDate(limitedInterval.Start), GraylogUrlDate(limitedInterval.End));
                var jsonLogs = await DownloadLogMessages(finalUrl);
                logs = ParseLogs<RequestLog>(jsonLogs);
                lastLog = logs.LastOrDefault() ?? lastLog;
                var groupedLogs = GroupRequestLogs(logs);
                _csvHandler.WriteDownloadedBatchToCsv(groupedLogs);
                limitedInterval.MoveLimitWindow(logs.LastOrDefault(), dateTimeInterval.End);
            }
            while (logs.Count == Limit || limitedInterval.End != dateTimeInterval.End);

            SaveLastLog(lastLog);
        }

        /// <summary>Creates filters for stream and source (service).</summary>
        /// <param name="streamId">Id of the stream in which to filter.</param>
        /// <param name="serviceName">Name of the service to filter.</param>
        /// <returns>List of filters for stream and source fields.</returns>
        private static List<FieldFilter> CreateFieldFilters(string streamId, string serviceName) {
            var fieldFilters = new List<FieldFilter> {new FieldFilter {Field = "streams", Value = streamId}};

            if (!string.IsNullOrEmpty(serviceName))
                fieldFilters.Add(new FieldFilter {Field = "source", Value = serviceName});

            return fieldFilters;
        }

        /// <summary>Downloads logs from Graylog.</summary>
        /// <param name="url">Graylog api url query.</param>
        /// <returns>List of requests in json format.</returns>
        private async Task<List<JToken>> DownloadLogMessages(string url) {
            var failDepth = 0;
            while (true) {
                try {
                    var response = await _client.GetAsync(url);
                    var logs = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(logs)["messages"]?.Children().ToList() ?? new List<JToken>();
                }
                catch (Exception e) {
                    _logger.LogError(e, "Failed to download log batch from Graylog.");
                    if (++failDepth == 3) // there will be 3 attempts to download before ending with an error
                        throw;
                }
            }
        }

        /// <summary>Parses requests in error format to C# representation.</summary>
        /// <typeparam name="TLog">Type of the log to parse.</typeparam>
        /// <param name="logMessages">Requests in json format.</param>
        /// <returns>Deserialized request logs.</returns>
        private List<TLog> ParseLogs<TLog>(IEnumerable<JToken> logMessages) where TLog : LogBase => logMessages.Select(jObj => jObj["message"]?.ToObject<TLog>()).Where(x => x != null).ToList();

        /// <summary>Groups error logs by service name and request type.</summary>
        /// <param name="logs">Sequence of error logs.</param>
        private void GroupErrorLogs(IEnumerable<ErrorLog> logs) {
            foreach (var log in logs) {
                if (!_errorLogs.ContainsKey(log.Source))
                    _errorLogs[log.Source] = new Dictionary<string, List<DateTime>>();

                if (!_errorLogs[log.Source].ContainsKey(log.RequestHandler))
                    _errorLogs[log.Source][log.RequestHandler] = new List<DateTime>();

                _errorLogs[log.Source][log.RequestHandler].Add(log.Timestamp);
            }
        }

        /// <summary>Groups request logs by service name and request type. It also counts errors in each request from grouped error logs.</summary>
        /// <param name="logs">Sequence of request logs.</param>
        /// <returns>Hierarchical dictionary of requests grouped by services and then by request types.</returns>
        private Dictionary<string, Dictionary<string, List<RequestDataPoint>>> GroupRequestLogs(IEnumerable<RequestLog> logs) {
            var data = new Dictionary<string, Dictionary<string, List<RequestDataPoint>>>();
            foreach (var log in logs) {
                if (!data.ContainsKey(log.Source))
                    data[log.Source] = new Dictionary<string, List<RequestDataPoint>>();

                if (!data[log.Source].ContainsKey(log.RequestHandler))
                    data[log.Source][log.RequestHandler] = new List<RequestDataPoint>();

                data[log.Source][log.RequestHandler].Add(
                    new RequestDataPoint {
                        Timestamp = log.Timestamp.AddHours(UtcHoursDiff), // Graylog is in UTC time, we want it in local
                        Duration = log.Duration,
                        Errors = FindRequestErrors(log)
                    });
            }

            return data;
        }

        /// <summary>Counts number of errors in a request.</summary>
        /// <param name="log">Request log.</param>
        /// <returns>Number of errors during the request.</returns>
        private int FindRequestErrors(RequestLog log) {
            if (!(_errorLogs.ContainsKey(log.Source) && _errorLogs[log.Source].ContainsKey(log.RequestHandler)))
                return 0;

            var before = _errorLogs[log.Source][log.RequestHandler].Count;
            _errorLogs[log.Source][log.RequestHandler].RemoveAll(x => x <= log.Timestamp && x >= log.Timestamp.AddMilliseconds(-log.Duration));
            return before - _errorLogs[log.Source][log.RequestHandler].Count;
        }

        /// <summary>Builds an url with filter query for downloading logs from Graylog.</summary>
        /// <param name="fields">Fields to download.</param>
        /// <param name="fieldFilters">Download filters.</param>
        /// <returns>Request url in which datetime interval is yet to be added, only by formatting though.</returns>
        private string BuildRequestQueryWithVariableDateInterval(IReadOnlyCollection<string> fields = null, IReadOnlyCollection<FieldFilter> fieldFilters = null) {
            var query = BuildSearchQuery(fieldFilters);

            query += $"&limit={Limit}";
            query += $"&sort=timestamp{UrlColon}asc";

            if (fields != null && fields.Count > 0)
                query += $"&fields={string.Join(",", fields)}";

            return $"{_baseUrl}?from={{0}}&to={{1}}&{query}";
        }

        /// <summary>Builds the search query part request url.</summary>
        /// <param name="fieldFilters">Field filters to be applied.</param>
        /// <returns>Search query part request url</returns>
        private string BuildSearchQuery(IReadOnlyCollection<FieldFilter> fieldFilters) {
            const string searchQuery = "query=";
            if (fieldFilters == null)
                return searchQuery + UrlEmptyString;

            var filters = fieldFilters.Select(x => $"{x.Field}{UrlColon}{UrlSpace}{x.Value}").ToList();
            return searchQuery + string.Join($"{UrlSpace}AND{UrlSpace}", filters);
        }

        /// <summary>Saves the timestamp of the last log.</summary>
        /// <param name="lastLog">Last downloaded log.</param>
        private void SaveLastLog(LogBase lastLog) {
            if (lastLog == null)
                return;

            var lastLogTimestamp = lastLog.Timestamp.AddHours(UtcHoursDiff);
            var lastSavedLogTimestamp = _csvHandler.GetLastDownloadedLogTimestamp();
            if (!lastSavedLogTimestamp.HasValue || lastLogTimestamp > lastSavedLogTimestamp)
                _csvHandler.SaveLastDownloadedLogTimestamp(lastLogTimestamp);
        }

        /// <summary>Transforms datetime to Graylog filter format.</summary>
        /// <param name="dt">Datetime to transform.</param>
        /// <returns>Graylog filter format of the datetime.</returns>
        private string GraylogUrlDate(DateTime dt) => dt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ").Replace(":", UrlColon);
    }
}