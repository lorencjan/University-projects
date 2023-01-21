using System.Threading.Tasks;
using Hotel.Command.Application.Reservations;
using Hotel.Command.Application.Reservations.Dtos;
using Hotel.ProjectionEngine.Client;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hotel.Command.Controllers;

/// <inheritdoc />
[ApiController, Route("api/v{version:apiVersion}/reservations")]
[ApiVersion("1")]
[ValidationExceptionFilter]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProjectionEngineClient _projectionEngineClient;

    public ReservationsController(IMediator mediator, IProjectionEngineClient projectionEngineClient)
    {
        _mediator = mediator;
        _projectionEngineClient = projectionEngineClient;
    }

    /// <summary>
    /// Creates a new database record of reservation from given values.
    /// </summary>
    /// <param name="reservation">Reservation values to be saved in the database</param>
    /// <returns>Id of the created reservation in the database</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody, BindRequired] ReservationDto reservation)
    {
        var id = await _mediator.Send(new CreateReservation { Reservation = reservation });
        await _projectionEngineClient.ProjectReservationsAsync();
        return Ok(id);
    }

    /// <summary>
    /// Updates the database record of the specified reservation entity with given values.
    /// </summary>
    /// <param name="id">Id of the reservation to be updated.</param>
    /// <param name="reservation">Reservation values to be updated in the database.</param>
    /// <returns></returns>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute][BindRequired] int id, [FromBody, BindRequired] ReservationUpdateDto reservation)
    {
        await _mediator.Send(new UpdateReservation { ReservationId = id, Reservation = reservation });
        await _projectionEngineClient.ProjectReservationsAsync();
        return Ok();
    }

    /// <summary>
    /// Deletes a reservation record with given id from the database.
    /// </summary>
    /// <param name="id">Id of the reservation to be deleted.</param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute, BindRequired] int id)
    {
        await _mediator.Send(new DeleteReservation { Id = id });
        await _projectionEngineClient.ProjectDeleteReservationAsync(id);
        return Ok();
    }
}