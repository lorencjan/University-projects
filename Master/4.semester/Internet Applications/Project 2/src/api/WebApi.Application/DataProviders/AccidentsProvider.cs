using Microsoft.Extensions.Options;
using WebApi.Application.Options;

namespace WebApi.Application.DataProviders;

public class AccidentsProvider : DataProvider
{
    public AccidentsProvider(IOptions<DatasetUrls> urls, IOptions<DatasetPaths> paths)
        : base(urls.Value.Accidents, paths.Value.Accidents) { }
}