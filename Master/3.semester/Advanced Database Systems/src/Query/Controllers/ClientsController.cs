using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Query.Application.Repository;

namespace Hotel.Query.Controllers;

/// <inheritdoc />
[ApiController, Route("api/v{version:apiVersion}/clients")]
[ApiVersion("1")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ClientsController : ControllerBase
{
    /// <summary>
    /// Gets clients from Cassandra database.
    /// </summary>
    /// <param name="email">Optional e-mail filter parameter.</param>
    /// <returns>List of clients</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClients([FromQuery] string email)
    {
        var result = string.IsNullOrEmpty(email) ? ClientRepository.GetAllClients() : ClientRepository.GetClientByEmail(email);
        return result.Any() ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Gets currently accommodated clients from the database.
    /// </summary>
    /// <returns>List of clients.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("clients/current")]
    public async Task<IActionResult> GetCurrentClients()
    {
        var result = ClientRepository.GetAllCurrentClients();
        return result.Any() ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Gets a client by id.
    /// </summary>
    /// <param name="id">Id of the client.</param>
    /// <returns>Single client if exists.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute][BindRequired] int id)
    {
        var result = ClientRepository.GetClientById(id);
        return result is null ? NotFound() : Ok(result);
    }
}