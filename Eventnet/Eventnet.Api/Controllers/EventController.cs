using System.Text.Json;
using Eventnet.DataAccess;
using Eventnet.Helpers;
using Eventnet.Models;
using Eventnet.Services;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace Eventnet.Controllers;

[Route("api/events")]
public class EventController : Controller
{
    private readonly IEventFilterService filterService;
    private readonly ApplicationDbContext dbContext;
    private readonly LinkGenerator linkGenerator;
    public const int MaxPageSize = 20;
    public const int DefaultPageSize = 10;

    public EventController(
        IEventFilterService filterService, 
        ApplicationDbContext dbContext, 
        LinkGenerator linkGenerator)
    {
        this.filterService = filterService;
        this.dbContext = dbContext;
        this.linkGenerator = linkGenerator;
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

    [HttpPost(Name = nameof(GetEvents))]
    public IActionResult GetEvents([FromBody] FilterEventsModel? filterModel, 
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = DefaultPageSize)
    {
        if (filterModel is null)
        {
            return BadRequest();
        }

        if (filterModel.Radius <= 0)
        {
            ModelState.AddModelError(nameof(FilterEventsModel.Radius), $"Radius should be positive, but was {filterModel.Radius}");
        }
        
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
            
        pageNumber = Normalize(pageNumber, 1);
        pageSize = Normalize(pageSize, 1, MaxPageSize);

        var filteredEvents = filterService.Filter(dbContext.Events, filterModel);
        var events = new PagedList<EventEntity>(filteredEvents, pageNumber, pageSize);
        var paginationHeader = events.ToPaginationHeader(GenerateEventsPageLink);
        
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));
        
        return Ok(events);
    }

    private string? GenerateEventsPageLink(int pageNumber, int pageSize)
    {
        return linkGenerator.GetUriByRouteValues(HttpContext, nameof(GetEvents), new { pageNumber, pageSize });
    }
    
    [HttpPost("create")]
    public IActionResult CreateEvent([FromBody] CreateEventModel createModel)
    {
        throw new NotImplementedException();
    }

    // TODO use format https://datatracker.ietf.org/doc/html/rfc6902
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

    private static int Normalize(int x, int min, int max = int.MaxValue) => Math.Max(min, Math.Min(x, max));
}