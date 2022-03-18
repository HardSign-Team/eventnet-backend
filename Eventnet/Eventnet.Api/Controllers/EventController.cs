using System.Text.Json;
using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.Helpers;
using Eventnet.Models;
using Eventnet.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace Eventnet.Controllers;

[Route("api/events")]
public class EventController : Controller
{
    public const int MaxPageSize = 20;
    public const int DefaultPageSize = 10;
    private readonly IEventFilterService filterService;
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly LinkGenerator linkGenerator;
    private readonly UserManager<UserEntity> userManager;
    private readonly IEventSaveService eventSaveService;

    public EventController(
        IEventFilterService filterService,
        ApplicationDbContext dbContext,
        IMapper mapper,
        LinkGenerator linkGenerator,
        UserManager<UserEntity> userManager,
        IEventSaveService eventSaveService)
    {
        this.filterService = filterService;
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.linkGenerator = linkGenerator;
        this.userManager = userManager;
        this.eventSaveService = eventSaveService;
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

        return Ok(mapper.Map<Event>(eventEntity));
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
            ModelState.AddModelError(nameof(FilterEventsModel.Radius),
                $"Radius should be positive, but was {filterModel.Radius}");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        pageNumber = NumberHelper.Normalize(pageNumber, 1);
        pageSize = NumberHelper.Normalize(pageSize, 1, MaxPageSize);

        var filteredEvents = filterService.Filter(dbContext.Events, filterModel);
        var events = new PagedList<EventEntity>(filteredEvents, pageNumber, pageSize);
        var paginationHeader = events.ToPaginationHeader(GenerateEventsPageLink);

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));

        return Ok(mapper.Map<IEnumerable<Event>>(events));
    }

    [HttpPost]
    public IActionResult CreateEvent([FromBody] CreateEventModel createModel)
    {
        if (createModel is null)
            return BadRequest();
        var createdEvent = mapper.Map<Event>(createModel);
        var files = createModel.Files;
        eventSaveService.Save(createdEvent, files);
        return Ok(new
        {
           StatusCode = StatusCodes.Status202Accepted
        });
    }

    [HttpGet("isCreated")]
    public IActionResult IsCreated(Guid id)
    {
        if (!eventSaveService.IsEventSaved(id, out var exception))
            return BadRequest(exception);
        return Ok();
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
        var eventEntity = dbContext.Events.FirstOrDefault(x => x.Id == eventId);
        if (eventEntity is null)
        {
            return NotFound();
        }

        dbContext.Events.Remove(eventEntity);
        await dbContext.SaveChangesAsync();

        return Ok(new { eventId });
    }

    [HttpGet("guid")]
    public IActionResult GenerateEventGuid()
    {
        var id = Guid.NewGuid();
        return Ok(id);
    }

    private string? GenerateEventsPageLink(int pageNumber, int pageSize)
    {
        return linkGenerator.GetUriByRouteValues(HttpContext, nameof(GetEvents), new { pageNumber, pageSize });
    }
}