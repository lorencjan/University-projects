using Newtonsoft.Json.Linq;

namespace WebApi.Application.DataProviders;

public abstract class DataProvider
{
    private readonly string _url;
    private readonly string _path;

    protected DataProvider(string url, string path) {
        _url = url;
        _path = path;
    }

    public async Task<byte[]> Retrieve() => await File.ReadAllBytesAsync(_path);

    public async Task Refresh() {
        var data = await Download();
        await Save(data);
    }

    private async Task<string> Download() {
        var client = new HttpClient();
        var result = await client.GetAsync(_url);
        return await result.Content.ReadAsStringAsync();
    }

    private Task Save(string data) {
        var fileInfo = new FileInfo(_path);
        fileInfo.Directory.Create();
        return File.WriteAllTextAsync(_path, data);
    }
}