using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Hotel.ProjectionEngine.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.ProjectionEngine.Controllers;

/// <inheritdoc />
[ApiController, Route("api/v{version:apiVersion}/projection-engine")]
[ApiVersion("1")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ProjectionEngineController : ControllerBase
{
    private readonly ProjectRepository _projectRepository;
    public ProjectionEngineController(ProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Projects clients from SQL database to Cassandra.
    /// </summary>
    /// <returns></returns>
    [HttpPost("clients")]
    public async Task<IActionResult> ProjectClients()
    {
        try
        {
            _projectRepository.ProjectClients();
            return Ok();
        }
        catch (System.Exception ex)
        {
            return Problem(ex?.Message);
        }

    }

    /// <summary>
    /// Projects reservations from SQL database to Cassandra.
    /// </summary>
    /// <returns></returns>
    [HttpPost("reservations")]
    public async Task<IActionResult> ProjectReservations()
    {
        try
        {
            _projectRepository.ProjectReservations();
            return Ok();
        }
        catch (System.Exception ex)
        {
            return Problem(ex?.Message);
        }
    }

    /// <summary>
    /// Projects rooms from SQL database to Cassandra.
    /// </summary>
    /// <returns></returns>
    [HttpPost("rooms")]
    public async Task<IActionResult> ProjectRooms()
    {
        try
        {
            _projectRepository.ProjectRooms();
            return Ok();
        }
        catch (System.Exception ex)
        {
            return Problem(ex?.Message);
        }
    }


    /// <summary>
    /// Projects rooms from SQL database to Cassandra.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ProjectUpdateRoom([Required]int id)
    {
        try
        {
            _projectRepository.ProjectRoomUpdate(id);
            return Ok(id);
        }
        catch (System.Exception ex)
        {
            return Problem(ex?.Message);
        }
    }

    [HttpDelete("deleteRoom/{id}")]
    public async Task<IActionResult> ProjectDeleteRoom([Required]int id)
    {
        try
        {
            _projectRepository.DeleteEntity(ProjectRepository.EntityNames.room, id);
            return Ok(id);
        }
        catch (System.Exception ex)
        {
            return Problem(ex?.Message);
        }
    }

    [HttpDelete("deleteClient/{id}")]
    public async Task<IActionResult> ProjectDeleteClient([Required] int id)
    {
        try
        {
            _projectRepository.DeleteEntity(ProjectRepository.EntityNames.client, id);
            return Ok(id);
        }
        catch (System.Exception ex)
        {
            return Problem(ex?.Message);
        }
    }

    [HttpDelete("deleteReservation/{id}")]
    public async Task<IActionResult> ProjectDeleteReservation([Required] int id)
    {
        try
        {
            _projectRepository.DeleteEntity(ProjectRepository.EntityNames.reservation, id);
            return Ok(id);
        }
        catch (System.Exception ex)
        {
            return Problem(ex?.Message);
        }
    }

}