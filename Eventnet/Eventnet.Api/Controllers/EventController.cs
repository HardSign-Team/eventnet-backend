using Eventnet.DataAccess;
using Eventnet.Models;
using Microsoft.AspNetCore.Mvc;

namespace Eventnet.Controllers;

[Route("api/events")]
public class EventController : Controller
{
    private readonly ApplicationDbContext dbContext;
    public EventController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet("{eventId:guid}")]
    public IActionResult GetEventById(Guid eventId)
    {
        if (Guid.Empty == eventId)
        {
            ModelState.AddModelError(nameof(eventId), $"{nameof(eventId)} should not be empty");
            return UnprocessableEntity(ModelState);
        }

        var eventEntity = dbContext.Events.FirstOrDefault(x => x.Id == eventId);
        if (eventEntity is null)
        {
            return NotFound();
        }

        return Ok(eventEntity);
    }

    [HttpPost]
    public IActionResult GetEvents([FromQuery] int pageNumber, [FromQuery] int pageSize,
        [FromBody] RequestEventsModel request)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("create")]
    public IActionResult CreateEvent([FromBody] CreateEventModel createModel)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{eventId:guid}")]
    public IActionResult UpdateEvent(Guid eventId, [FromBody] UpdateEventModel updateModel)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{eventId:guid}")]
    public IActionResult DeleteEvent(Guid eventId)
    {
        throw new NotImplementedException();
    }
}


