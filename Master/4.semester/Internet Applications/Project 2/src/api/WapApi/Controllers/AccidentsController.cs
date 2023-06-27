using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DataProviders;

namespace WapApi.Controllers;

[Route("accidents")]
public class AccidentsController : DataControllerBase<AccidentsProvider>
{
    public AccidentsController(AccidentsProvider provider) : base(provider) { }
}