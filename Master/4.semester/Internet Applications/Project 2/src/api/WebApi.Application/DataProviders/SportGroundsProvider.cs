using Microsoft.Extensions.Options;
using WebApi.Application.Options;

namespace WebApi.Application.DataProviders;

public class SportGroundsProvider : DataProvider
{
    public SportGroundsProvider(IOptions<DatasetUrls> urls, IOptions<DatasetPaths> paths)
        : base(urls.Value.SportGrounds, paths.Value.SportGrounds) { }
}