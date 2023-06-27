using WebApi.Application.Services;

namespace WapApi;

public class StartupService : IHostedService
{
    private readonly RefreshService _service;

    public StartupService(RefreshService service) {
        _service = service;
    }

    public Task StartAsync(CancellationToken cancellationToken) => _service.Refresh();

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}