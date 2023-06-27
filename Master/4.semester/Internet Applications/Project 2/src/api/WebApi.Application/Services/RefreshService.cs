using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApi.Application.DataProviders;

namespace WebApi.Application.Services;

public class RefreshService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RefreshService> _logger;

    public RefreshService(IServiceProvider serviceProvider, ILogger<RefreshService> logger) {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Refresh() {
        try {
            await _serviceProvider.GetRequiredService<AccidentsProvider>().Refresh();
            await _serviceProvider.GetRequiredService<SchoolsProvider>().Refresh();
            await _serviceProvider.GetRequiredService<SportGroundsProvider>().Refresh();
        }
        catch (Exception e) {
            _logger.LogError(e, "Failed to refresh all data. Maybe a download link is no longer valid.");
        }
    }
}