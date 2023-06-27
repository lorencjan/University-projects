using Microsoft.Extensions.Options;
using WebApi.Application.Options;

namespace WebApi.Application.DataProviders;

public class SchoolsProvider : DataProvider
{
    public SchoolsProvider(IOptions<DatasetUrls> urls, IOptions<DatasetPaths> paths)
        : base(urls.Value.Schools, paths.Value.Schools) { }
}