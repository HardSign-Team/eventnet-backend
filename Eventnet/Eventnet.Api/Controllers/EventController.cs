using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Eventnet.Api.Config;
using Eventnet.Api.Extensions;
using Eventnet.Api.Helpers;
using Eventnet.Api.Models.Events;
using Eventnet.Api.Models.Filtering;
using Eventnet.Api.Models.Pages;
using Eventnet.Api.Services;
using Eventnet.Api.Services.SaveServices;
using Eventnet.DataAccess;
using Eventnet.Domain.Events;
using Eventnet.Domain.Selectors;
using Eventnet.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Api.Controllers;

[Route("api/events")]
public class EventController : Controller
{
    private const int Timeout = 20;
    private static readonly string[] SupportedContentTypes = { "image/bmp", "image/png", "image/jpeg" };
    private readonly ApplicationDbContext dbContext;
    private readonly IEventSaveService eventSaveService;
    private readonly CurrentUserService currentUserService;
    private readonly LinkGenerator linkGenerator;
    private readonly EventsFilterService eventsFilterService;
    private readonly RabbitMqConfig rabbitMqConfig;
    private readonly IMapper mapper;

    public EventController(
        ApplicationDbContext dbContext,
        IMapper mapper,
        IEventSaveService eventSaveService,
        CurrentUserService currentUserService,
        LinkGenerator linkGenerator,
        EventsFilterService eventsFilterService,
        RabbitMqConfig rabbitMqConfig)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.eventSaveService = eventSaveService;
        this.currentUserService = currentUserService;
        this.linkGenerator = linkGenerator;
        this.eventsFilterService = eventsFilterService;
        this.rabbitMqConfig = rabbitMqConfig;
    }

    [HttpGet("{eventId:guid}")]
    [Produces(typeof(EventViewModel))]
    public async Task<IActionResult> GetEventById(Guid eventId)
    {
        if (Guid.Empty == eventId)
        {
            ModelState.AddModelError(nameof(eventId), $"{nameof(eventId)} should not be empty");
            return UnprocessableEntity(ModelState);
        }

        var entity = await mapper.ProjectTo<EventViewModel>(dbContext.Events)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == eventId);
        if (entity is null)
            return NotFound();

        return Ok(entity);
    }

    [HttpPost("full")]
    [Produces(typeof(List<EventViewModel>))]
    public async Task<IActionResult> GetEventsByIds([FromBody] EventIdsListModel? listModel)
    {
        if (listModel is null)
            return BadRequest($"{nameof(listModel)} was null");

        var ids = listModel.Ids;
        var viewModels = await dbContext.Events
            .ProjectTo<EventViewModel>(mapper.ConfigurationProvider)
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
        return Ok(viewModels);
    }

    [Authorize]
    [HttpGet("my")]
    [Produces(typeof(List<EventViewModel>))]
    public async Task<IActionResult> GetUserEvents()
    {
        var user = await currentUserService.GetCurrentUserAsync();
        if (user is null)
            return Unauthorized();

        var viewModels = await dbContext.Events
            .ProjectTo<EventViewModel>(mapper.ConfigurationProvider)
            .AsNoTracking()
            .Where(x => x.OwnerId == user.Id)
            .ToListAsync();
        return Ok(viewModels);
    }

    [HttpGet("search/name/{eventName}")]
    [Produces(typeof(List<EventNameViewModel>))]
    public IActionResult GetEventsByName(string? eventName, [FromQuery(Name = "m")] int maxCount = 10)
    {
        eventName = eventName?.Trim();
        switch (eventName)
        {
            case null:
                return BadRequest($"{nameof(eventName)} undefined");
            case "":
                return UnprocessableEntity($"Expected {nameof(eventName)} is non-empty string");
        }

        var selector = new EventsByNameSelector(eventName);
        var query = mapper.ProjectTo<EventName>(dbContext.Events).AsNoTracking();
        var result = selector.Select(query.AsEnumerable(), maxCount);
        var viewModel = mapper.Map<List<EventNameViewModel>>(result);
        return Ok(viewModel);
    }

    [HttpGet(Name = nameof(GetEvents))]
    [Produces(typeof(List<EventLocationViewModel>))]
    public IActionResult GetEvents(
        [FromQuery(Name = "f")] string? filterModelBase64,
        [FromQuery(Name = "p")] int pageNumber = 1,
        [FromQuery(Name = "ps")] int pageSize = 10)
    {
        if (filterModelBase64 is null)
            return BadRequest($"{nameof(filterModelBase64)} was null");
        if (!EventsFilterModel.TryParse(filterModelBase64, out var filterModel))
            return BadRequest("Cannot parse filter model");
        if (!TryValidateModel(filterModel))
            return UnprocessableEntity(ModelState);

        var pageInfo = new PageInfo(pageNumber, pageSize);
        var query = mapper.ProjectTo<Event>(dbContext.Events).AsNoTracking();
        var events = eventsFilterService.GetEvents(query.AsEnumerable(), filterModel, pageInfo);
        var paginationHeader = events.ToPaginationHeader(GenerateEventsPageLink(filterModelBase64));

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));

        return Ok(mapper.Map<List<EventLocationViewModel>>(events));
    }

    [Authorize]
    [HttpGet("request-event-creation")]
    [Produces(typeof(Guid))]
    public IActionResult RequestEventCreation() => Ok(Guid.NewGuid());

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromForm] CreateEventModel? createModel)
    {
        if (createModel is null)
            return UnprocessableEntity();

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var user = await currentUserService.GetCurrentUserAsync();
        if (user is null)
            return Unauthorized();

        var photos = createModel.Photos ?? Array.Empty<IFormFile>();
        if (!IsContentTypesSupported(photos))
            return BadRequest("Not supported ContentType");

        if (!rabbitMqConfig.IsPhotosSizeLessThanRecommended(photos))
            return BadRequest(
                $"Too large images. Recommended size of all images is {rabbitMqConfig.RecommendedMessageSizeInMb()}Mb.");

        var eventId = createModel.EventId;
        var isSaved = await dbContext.Events.AnyAsync(x => x.Id == eventId);
        if (eventSaveService.IsHandling(eventId) || isSaved)
            return BadRequest("One event id provided two times");

        var eventInfo = mapper.Map<EventInfo>(createModel) with { OwnerId = user.Id };
        await eventSaveService.RequestSave(eventInfo, photos);

        return Accepted();
    }

    [Authorize]
    [HttpGet("is-created")]
    public IActionResult IsCreated(Guid id)
    {
        var (saveStatus, exception) = eventSaveService.GetSaveEventResult(id);
        return saveStatus switch
        {
            EventSaveStatus.Saved => Ok(),
            EventSaveStatus.NotSavedDueToUserError => BadRequest(exception),
            EventSaveStatus.NotSavedDueToServerError => BadRequest(exception),
            EventSaveStatus.InProgress => Accepted(Timeout),
            _ => throw new ArgumentOutOfRangeException($"Unknown SaveState {saveStatus}")
        };
    }

    // TODO use format https://datatracker.ietf.org/doc/html/rfc6902
    [HttpPatch("{eventId:guid}")]
    public IActionResult UpdateEvent(Guid eventId, [FromBody] UpdateEventModel updateModel) =>
        throw new NotImplementedException();

    [Authorize]
    [HttpDelete("{eventId:guid}")]
    [Produces(typeof(Guid))]
    public async Task<IActionResult> DeleteEvent(Guid eventId)
    {
        var eventEntity = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        if (eventEntity is null)
            return NotFound();

        dbContext.Events.Remove(eventEntity);
        await dbContext.SaveChangesAsync();

        return Ok(eventId);
    }

    private static bool IsContentTypesSupported(IFormFile[] files)
    {
        var contentTypes = files.Select(photo => photo.ContentType);
        return contentTypes.All(SupportedContentTypes.Contains);
    }

    private Func<int, int, string?> GenerateEventsPageLink(string filterModelBase64)
    {
        return (pageNumber, pageSize) => linkGenerator.GetUriByRouteValues(HttpContext,
            nameof(GetEvents),
            new { f = filterModelBase64, p = pageNumber, ps = pageSize });
    }
}