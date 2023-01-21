using System.Threading.Tasks;
using Hotel.Command.Application.Rooms;
using Hotel.Command.Application.Rooms.Dtos;
using Hotel.ProjectionEngine.Client;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hotel.Command.Controllers;

/// <inheritdoc />
[ApiController, Route("api/v{version:apiVersion}/rooms")]
[ApiVersion("1")]
[ValidationExceptionFilter]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class RoomsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProjectionEngineClient _projectionEngineClient;

    public RoomsController(IMediator mediator, IProjectionEngineClient projectionEngineClient)
    {
        _mediator = mediator;
        _projectionEngineClient = projectionEngineClient;
    }

    /// <summary>
    /// Creates a new database record of room from given values.
    /// </summary>
    /// <param name="room">Room values to be saved in the database</param>
    /// <returns>Id of the created room in the database</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody, BindRequired] RoomDto room)
    {
        var id = await _mediator.Send(new CreateRoom { Room = room });
        await _projectionEngineClient.ProjectRoomsAsync();
        return Ok(id);
    }

    /// <summary>
    /// Updates the database record of the specified room entity with given values.
    /// </summary>
    /// <param name="id">Id of the room to be updated.</param>
    /// <param name="room">Room values to be updated in the database.</param>
    /// <returns></returns>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute][BindRequired] int id, [FromBody, BindRequired] RoomUpdateDto room)
    {
        await _mediator.Send(new UpdateRoom { RoomId = id, Room = room });
        await _projectionEngineClient.ProjectUpdateRoomAsync(id);
        return Ok();
    }

    /// <summary>
    /// Deletes a room record with given id from the database.
    /// </summary>
    /// <param name="id">Id of the room to be deleted.</param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute, BindRequired] int id)
    {
        await _mediator.Send(new DeleteRoom { Id = id });
        await _projectionEngineClient.ProjectDeleteRoomAsync(id);
        return Ok();
    }
}