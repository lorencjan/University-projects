using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DataProviders;

namespace WapApi.Controllers;

[Route("schools")]
public class SchoolsController : DataControllerBase<SchoolsProvider>
{
    public SchoolsController(SchoolsProvider provider) : base(provider) { }
}