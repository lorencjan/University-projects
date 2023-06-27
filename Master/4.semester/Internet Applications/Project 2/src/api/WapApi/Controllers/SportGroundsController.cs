using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DataProviders;

namespace WapApi.Controllers;

[Route("sport-grounds")]
public class SportGroundsController : DataControllerBase<SportGroundsProvider>
{
    public SportGroundsController(SportGroundsProvider provider) : base(provider) { }
}