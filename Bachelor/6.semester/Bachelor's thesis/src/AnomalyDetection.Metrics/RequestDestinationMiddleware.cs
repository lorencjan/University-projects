// File: RequestDestinationMiddleware.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Metrics

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace YSoft.Rqa.AnomalyDetection.Metrics
{
    /// <summary>
    /// Adds information about request handler in the form of {controller-action} to every log within the application.
    /// Does nothing if it's not an api call.
    /// </summary>
    public class RequestDestinationMiddleware
    {
        private readonly ILogger<RequestDestinationMiddleware> _logger;
        private readonly RequestDelegate _next;

        public RequestDestinationMiddleware(RequestDelegate next, ILogger<RequestDestinationMiddleware> logger) {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context) {
            var routeData = context.GetRouteData()?.Values;
            if (routeData != null) {
                var hasController = routeData.ContainsKey("controller") && routeData["controller"] != null;
                var hasAction = routeData.ContainsKey("action") && routeData["action"] != null;
                if (hasController && hasAction) {
                    var requestHandler = $"{routeData["controller"]}-{routeData["action"]}";
                    using (_logger.BeginScope(new Dictionary<string, object> {["requestHandler"] = requestHandler}))
                        await _next.Invoke(context);
                }
                else {
                    await _next.Invoke(context);
                }
            }
            else {
                await _next.Invoke(context);
            }
        }
    }
}