using System.Threading.Tasks;
using Hotel.Command.Application.Clients;
using Hotel.Command.Application.Clients.Dtos;
using Hotel.ProjectionEngine.Client;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hotel.Command.Controllers;

/// <inheritdoc />
[ApiController, Route("api/v{version:apiVersion}/clients")]
[ApiVersion("1")]
[ValidationExceptionFilter]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProjectionEngineClient _projectionEngineClient;

    public ClientsController(IMediator mediator, IProjectionEngineClient projectionEngineClient)
    {
        _mediator = mediator;
        _projectionEngineClient = projectionEngineClient;
    }

    /// <summary>
    /// Creates a new database record of client from given values.
    /// </summary>
    /// <param name="client">Client values to be saved in the database</param>
    /// <returns>Id of the created client in the database</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody, BindRequired] ClientDto client)
    {
        var id = await _mediator.Send(new CreateClient { Client = client });
        await _projectionEngineClient.ProjectClientsAsync();
        return Ok(id);
    }

    /// <summary>
    /// Updates the database record of the specified client entity with given values.
    /// </summary>
    /// <param name="id">Id of the client to be updated.</param>
    /// <param name="client">Client values to be updated in the database.</param>
    /// <returns></returns>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute][BindRequired] int id, [FromBody, BindRequired] UpdateClientDto client)
    {
        await _mediator.Send(new UpdateClient { ClientId = id, Client = client });
        await _projectionEngineClient.ProjectClientsAsync();
        return Ok();
    }

    /// <summary>
    /// Deletes a client record with given id from the database.
    /// </summary>
    /// <param name="id">Id of the client to be deleted.</param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute, BindRequired] int id)
    {
        await _mediator.Send(new DeleteClient { Id = id });
        await _projectionEngineClient.ProjectDeleteClientAsync(id);
        return Ok();
    }
}