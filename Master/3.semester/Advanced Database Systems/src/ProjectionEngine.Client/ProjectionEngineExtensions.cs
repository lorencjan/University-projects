using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hotel.ProjectionEngine.Client;

public static class ProjectionEngineExtensions
{
    /// <summary>
    /// Registers ProjectionEngine client into service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="action"></param>
    public static void AddProjectionEngine(this IServiceCollection services, IConfiguration configuration, Action<ProjectionEngineOptions> action = null)
    {
        services.Configure<ProjectionEngineOptions>(configuration.GetSection(nameof(ProjectionEngineOptions)).Bind);
        if (action != null)
            services.Configure(action);

        services.AddHttpClient<IProjectionEngineClient, ProjectionEngineClient>(ConfigureClient);
    }

    private static void ConfigureClient(IServiceProvider provider, HttpClient client)
    {
        using var serviceScope = provider.CreateScope();
        var uriString = serviceScope.ServiceProvider.GetRequiredService<IOptionsSnapshot<ProjectionEngineOptions>>().Value.Uri;
        if (string.IsNullOrWhiteSpace(uriString))
            throw new Exception($"Uri must be set in {nameof(ProjectionEngineOptions)} to use ProjectionEngine services.");

        if (!Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            throw new Exception($"Uri inside of {nameof(ProjectionEngineOptions)} is not valid.");

        client.BaseAddress = uri;
    }
}