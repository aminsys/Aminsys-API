using System.ComponentModel.DataAnnotations;
using aminsys_api.Model;
using aminsys_api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace aminsys_api.Controllers;

[ApiController]
[Route("[controller]")]
public class PeopleController : ControllerBase
{
    private readonly PeopleService _peopleService;

    public PeopleController(PeopleService peopleService) => _peopleService = peopleService;

    [HttpGet]
    [Route("/people")]
    [SwaggerOperation(
        Summary = "Get people",
        Description = "Get all people in database"
    )]
    [SwaggerResponse(200, "Success")]
    [SwaggerResponse(400, "Bad request")]
    public async Task<ActionResult<IEnumerable<People>>> GetPeople() => await _peopleService.GetAsync();

    [HttpGet]
    [Route("/person")]
    [SwaggerOperation(
        Summary = "Get person",
        Description = "Get a person with a certain ID"
    )]
    [SwaggerResponse(200, "Success")]
    [SwaggerResponse(400, "Bad request")]
    public async Task<ActionResult<People>> GetPerson([FromQuery][Required] int id)
    {
      var person = await _peopleService.GetAsync(id);
      if(person is null){
        return NotFound();
      }
      return person;
    }

    [HttpGet]
    [Route("/peoplebydate")]
    [SwaggerOperation(
        Summary = "Get people in dates interval",
        Description = "Get people's data with a certain dates interval."
    )]
    [SwaggerResponse(200, "Success")]
    [SwaggerResponse(400, "Bad request")]
    public async Task<ActionResult<IEnumerable<People>>> GetPeopleByDate([FromQuery][Required] DateTime fromDate, [FromQuery][Required] DateTime toDate) =>
      await _peopleService.GetAsync(fromDate, toDate);
}
