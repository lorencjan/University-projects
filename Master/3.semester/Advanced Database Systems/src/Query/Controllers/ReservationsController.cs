using System.Linq;
using System.Threading.Tasks;
using Hotel.Query.Application.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hotel.Query.Controllers;

/// <inheritdoc />
[ApiController, Route("api/v{version:apiVersion}/reservations")]
[ApiVersion("1")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ReservationsController : ControllerBase
{
    /// <summary>
    /// Gets reservations from Cassandra database.
    /// </summary>
    /// <returns>List of reservations</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservations()
    {
        var result = ReservationRepository.GetAllReservations();
        return result.Any() ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Gets a reservation by id.
    /// </summary>
    /// <param name="id">Id of the reservation.</param>
    /// <returns>Single reservation if exists.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute][BindRequired] int id)
    {
        var result = ReservationRepository.GetReservationById(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Gets all reservations of client
    /// </summary>
    /// <param name="id">Client ID.</param>
    /// <returns>List of all reservations of given client.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("client/{id}")]
    public async Task<IActionResult> GetClientReservations([FromRoute][BindRequired] int id)
    {
        var result = ReservationRepository.GetAllReservationsOfClient(id);
        return result.Any() ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Gets all reservations of room
    /// </summary>
    /// <param name="id">Room ID.</param>
    /// <returns>List of all reservations of given room.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("room/{id}")]
    public async Task<IActionResult> GetRoomReservations([FromRoute][BindRequired] int id)
    {
        var result = ReservationRepository.GetAllRoomReservations(id);
        return result.Any() ? Ok(result) : NotFound();
    }
}