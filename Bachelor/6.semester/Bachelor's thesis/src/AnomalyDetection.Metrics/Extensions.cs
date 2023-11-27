// File: Extensions.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Metrics

using Microsoft.AspNetCore.Builder;

namespace YSoft.Rqa.AnomalyDetection.Metrics
{
    public static class Extensions
    {
        /// <summary>
        /// Registers middlewares measuring metrics for anomaly detection.
        /// Requires UseRouting() registration before it.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns>The IApplicationBuilder instance.</returns>
        public static IApplicationBuilder UseAnomalyDetection(this IApplicationBuilder applicationBuilder) {
            applicationBuilder.UseMiddleware<RequestDestinationMiddleware>();
            applicationBuilder.UseMiddleware<RequestDurationMiddleware>();
            return applicationBuilder;
        }
    }
}