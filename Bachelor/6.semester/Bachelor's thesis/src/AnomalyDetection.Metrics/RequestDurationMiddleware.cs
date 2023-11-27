// File: RequestDurationMiddleware.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Metrics

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace YSoft.Rqa.AnomalyDetection.Metrics
{
    /// <summary>Logs the duration of api requests.</summary>
    public class RequestDurationMiddleware
    {
        private readonly ILogger<RequestDurationMiddleware> _logger;
        private readonly RequestDelegate _next;

        public RequestDurationMiddleware(RequestDelegate next, ILogger<RequestDurationMiddleware> logger) {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context) {
            var watch = Stopwatch.StartNew();
            try {
                await _next.Invoke(context);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Unhandled exception occurred during execution of {context.Request.Path}");
                throw;
            }
            finally {
                watch.Stop();
                _logger.LogInformation("Service request " + context.Request.Path + " took {duration} ms.", watch.Elapsed.TotalMilliseconds);
            }
        }
    }
}