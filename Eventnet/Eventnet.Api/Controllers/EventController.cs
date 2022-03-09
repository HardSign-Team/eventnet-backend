using Eventnet.Models;
using Microsoft.AspNetCore.Mvc;

namespace Eventnet.Controllers;

[Route("api/events")]
public class EventController : Controller
{
    public EventController() {}

    [HttpGet("{eventId:guid}")]
    public IActionResult GetEventById(Guid eventId)
    {
        throw new NotImplementedException();
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


