using Quartz;
using WebApi.Application.Services;

namespace WebApi.Application.Jobs;

public class RefreshJob : IJob
{
    private readonly RefreshService _service;

    public RefreshJob(RefreshService service) {
        _service = service;
    }

    public Task Execute(IJobExecutionContext context) => _service.Refresh();
}