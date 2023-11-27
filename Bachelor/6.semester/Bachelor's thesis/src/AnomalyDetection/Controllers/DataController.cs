// File: DataController.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YSoft.Rqa.AnomalyDetection.Data.Enums;
using YSoft.Rqa.AnomalyDetection.Data.Model.Graylog;
using YSoft.Rqa.AnomalyDetection.Data.Services;

namespace YSoft.Rqa.AnomalyDetection.Controllers
{
    /// <inheritdoc />
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class DataController : ControllerBase
    {
        private readonly CsvHandler _csvHandler;
        private readonly GraylogProvider _graylogProvider;

        public DataController(GraylogProvider graylogProvider, CsvHandler csvHandler) {
            _graylogProvider = graylogProvider;
            _csvHandler = csvHandler;
        }

        /// <summary>
        /// Downloads new logs from Graylog and adds them to the adequate csv files.
        /// The download window is from last downloaded log timestamp to now.
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(GetRecentData))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRecentData() {
            var lastLogTimestamp = _csvHandler.GetLastDownloadedLogTimestamp();
            if (!lastLogTimestamp.HasValue)
                return BadRequest("No reference data has been downloaded yet.");

            var interval = new DateTimeInterval(lastLogTimestamp.Value.AddHours(-_graylogProvider.UtcHoursDiff), DateTime.UtcNow);
            await _graylogProvider.DownloadLogs(interval);
            return Ok();
        }

        /// <summary>
        /// Downloads logs from Graylog and stores them in adequate csv files.
        /// If there already exist csv data files, they will get replaced.
        /// </summary>
        /// <param name="intervalLength">Length of the interval which to download in days.</param>
        /// <param name="serviceName">If specified, it gets only logs from that service.</param>
        /// <returns></returns>
        [HttpGet(nameof(GetReferenceData))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReferenceData(int intervalLength = 7, string serviceName = null) {
            _csvHandler.RemoveExistingData(DataType.Reference, serviceName);
            var interval = new DateTimeInterval(DateTime.UtcNow.AddDays(-intervalLength), DateTime.UtcNow);
            await _graylogProvider.DownloadLogs(interval, serviceName);
            return Ok();
        }

        /// <summary>Deletes specified data.</summary>
        /// <param name="dataType">Type of data to delete. Reference/downloaded but not yet processed/both.</param>
        /// <param name="serviceName">If specified, it deletes only data of the service.</param>
        /// <returns></returns>
        [HttpDelete(nameof(DeleteData))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteData(DataType dataType = DataType.All, string serviceName = null) {
            _csvHandler.RemoveExistingData(dataType, serviceName);
            return Ok();
        }
    }
}