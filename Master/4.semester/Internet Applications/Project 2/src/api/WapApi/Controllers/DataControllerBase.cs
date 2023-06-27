using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DataProviders;

namespace WapApi.Controllers;

[ApiController]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class DataControllerBase<T> : ControllerBase where T : DataProvider
{
    private readonly T _provider;

    public DataControllerBase(T provider) {
        _provider = provider;
    }

    [HttpGet]
    public async Task<IActionResult> GetSchoolData() {
        var bytes = await _provider.Retrieve();
        return new FileContentResult(bytes, "text/json");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshSchoolData() {
        await _provider.Refresh();
        return new OkResult();
    }
}