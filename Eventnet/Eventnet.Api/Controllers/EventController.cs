using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    [HttpGet(Name = nameof(GetEvents))]
    public IActionResult GetEvents(
        [FromQuery(Name = "f")] string? filterModelBase64,
        [FromQuery(Name = "p")] int pageNumber = 1, 
        [FromQuery(Name ="ps")] int pageSize = DefaultPageSize)
    {
        var filterModel = ParseEventsFilterModel(filterModelBase64);
        if (filterModel is null)
        {
            return BadRequest();
        }

        TryValidateModel(filterModel);
        
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
        var paginationHeader = events
                .ToPaginationHeader((p, ps) => GenerateEventsPageLink(filterModelBase64, p, ps));

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));

        return Ok(mapper.Map<IEnumerable<Event>>(events));
    }

    private EventsFilterModel? ParseEventsFilterModel(string? base64Model)
    {
        if (base64Model is null)
        {
            return null;
        }

        try
        {
            var bytes = Convert.FromBase64String(base64Model);
            var json = Encoding.Default.GetString(bytes);
            return JsonSerializer.Deserialize<EventsFilterModel>(json);
        }
        catch (Exception)
        {
            return null;
        }
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

    private string? GenerateEventsPageLink(string filterModelBase64, int pageNumber, int pageSize)
    {
        var values = new { f = filterModelBase64, p = pageNumber, ps = pageSize };
        return linkGenerator.GetUriByRouteValues(HttpContext, nameof(GetEvents), values);
    }
}