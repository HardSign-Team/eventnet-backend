using System.Text.Json;
using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.Helpers;
using Eventnet.Models;
using Eventnet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Eventnet.Controllers;

[Route("api/events")]
public class EventController : Controller
{
    public const int MaxPageSize = 20;
    public const int DefaultPageSize = 10;
    private readonly IEventFilterMapper filterMapper;
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly LinkGenerator linkGenerator;

    public EventController(
        IEventFilterMapper filterMapper,
        ApplicationDbContext dbContext,
        IMapper mapper,
        LinkGenerator linkGenerator)
    {
        this.filterMapper = filterMapper;
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.linkGenerator = linkGenerator;
    }

    [HttpGet("{eventId:guid}")]
    public async Task<IActionResult> GetEventById(Guid eventId)
    {
        if (Guid.Empty == eventId)
        {
            ModelState.AddModelError(nameof(eventId), $"{nameof(eventId)} should not be empty");
            return UnprocessableEntity(ModelState);
        }

        var eventEntity = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        if (eventEntity is null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<Event>(eventEntity));
    }

    [HttpPost(Name = nameof(GetEvents))]
    public IActionResult GetEvents([FromBody] EventsFilterModel? filterModel,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = DefaultPageSize)
    {
        if (filterModel is null)
        {
            return BadRequest();
        }

        ValidateFilterModel(filterModel);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        pageNumber = NumberHelper.Normalize(pageNumber, 1);
        pageSize = NumberHelper.Normalize(pageSize, 1, MaxPageSize);

        var query = dbContext.Events.AsNoTracking().AsEnumerable();
        var filter = filterMapper.Map(filterModel);
        var filteredEvents = filter.Filter(query);

        var events = new PagedList<EventEntity>(filteredEvents, pageNumber, pageSize);
        var paginationHeader = events.ToPaginationHeader(GenerateEventsPageLink);

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));

        return Ok(mapper.Map<IEnumerable<Event>>(events));
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
    public async Task<IActionResult> DeleteEvent(Guid eventId)
    {
        var eventEntity = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        if (eventEntity is null)
        {
            return NotFound();
        }

        dbContext.Events.Remove(eventEntity);
        await dbContext.SaveChangesAsync();

        return Ok(new { eventId });
    }

    private void ValidateFilterModel(EventsFilterModel eventsFilterModel)
    {
        if (eventsFilterModel.RadiusLocation is { } radiusLocation)
        {
            var radius = radiusLocation.Radius;
            if (radius <= 0)
            {
                ModelState.AddModelError(nameof(radius), $"Radius should be positive, but was {radius}");
            }
        }
    }

    private string? GenerateEventsPageLink(int pageNumber, int pageSize)
    {
        return linkGenerator.GetUriByRouteValues(HttpContext, nameof(GetEvents), new { pageNumber, pageSize });
    }
}