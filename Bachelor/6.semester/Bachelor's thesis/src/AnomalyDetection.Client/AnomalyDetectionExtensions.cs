// File: AnomalyDetectionExtensions.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Client

using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;

namespace YSoft.Rqa.AnomalyDetection.Client
{
    public static class AnomalyDetectionExtensions
    {
        /// <summary>Registers YSoft RQA Anomaly Detection clients into a service collection.</summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <param name="action"></param>
        public static void AddAnomalyDetectionClient(this IServiceCollection serviceCollection, IConfiguration configuration, Action<AnomalyDetectionOptions> action = null) {
            serviceCollection.Configure<AnomalyDetectionOptions>(configuration.GetSection(nameof(AnomalyDetectionOptions)).Bind);
            if (action != null)
                serviceCollection.Configure(action);

            serviceCollection.AddHttpClient<IAnomalyDetectionClient, AnomalyDetectionClient>(ConfigureClient)
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(i * 500)));
        }

        private static void ConfigureClient(IServiceProvider provider, HttpClient client) {
            using var serviceScope = provider.CreateScope();
            var uriString = serviceScope.ServiceProvider.GetRequiredService<IOptionsSnapshot<AnomalyDetectionOptions>>().Value.Uri;
            if (string.IsNullOrWhiteSpace(uriString))
                throw new Exception($"Uri must be set in {nameof(AnomalyDetectionOptions)} to use Devices service.");

            if (!Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
                throw new Exception($"Uri inside of {nameof(AnomalyDetectionOptions)} is not valid.");

            client.BaseAddress = uri;
        }
    }
}