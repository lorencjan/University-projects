using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Query.Application.Repository;

namespace Hotel.Query.Controllers;

/// <inheritdoc />
[ApiController, Route("api/v{version:apiVersion}/room")]
[ApiVersion("1")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class RoomController : ControllerBase
{
    /// <summary>
    /// Gets a room by id.
    /// </summary>
    /// <param name="id">Id of the room.</param>
    /// <returns>Single room if exists.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute][BindRequired] int id)
    {
        var result = RoomRepository.GetRoomById(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Gets a rooms by multiple params
    /// </summary>
    /// <param name="floor">patro</param>
    /// <param name="costPerNight">cena za noc</param>
    /// <param name="numberOfBeds">pocet posteli</param>
    /// <param name="numberOfSideBeds">pocet pristylek</param>
    /// <returns>Single room if exists.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRooms(int? floor = null, int? costPerNight = null, int? numberOfBeds = null, int? numberOfSideBeds = null)
    {
        var result = RoomRepository.GetAllRooms(floor, costPerNight, numberOfBeds, numberOfSideBeds);
        return result.Any() ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Gets all uncleaned rooms
    /// </summary>
    /// <returns>List of all uncleaned roooms.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("room/uncleaned")]
    public async Task<IActionResult> GetUncleanedRooms()
    {
        var result = RoomRepository.GetUncleanedRooms();
        return result.Any() ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Gets all available rooms 
    /// </summary>
    /// <param name="fromDate">Arrival date. format: yyyy-mm-dd</param>
    /// <param name="toDate">Departure date. format: yyyy-mm-dd</param>
    /// <returns>List of all available rooms in range of dates.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("room/available")]
    public async Task<IActionResult> GetAvailableRooms(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var result = RoomRepository.AvailableRooms(fromDate, toDate);
        return result.Any() ? Ok(result) : NotFound();
    }
}