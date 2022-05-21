using Eventnet.Api.Helpers;
using Eventnet.Api.Models.Filtering;
using Eventnet.Api.Models.Pages;
using Eventnet.Api.Services.Filters;
using Eventnet.Domain.Events;
using X.PagedList;

namespace Eventnet.Api.Services;

public class EventsFilterService
{
    private readonly IEventFilterMapper filterMapper;

    public EventsFilterService(IEventFilterMapper filterMapper)
    {
        this.filterMapper = filterMapper;
    }

    public PagedList<Event> GetEvents(IEnumerable<Event> query, EventsFilterModel filterModel, PageInfo page)
    {
        var (pageNumber, pageSize) = page;
        pageNumber = NumberHelper.Normalize(pageNumber, 1);
        pageSize = NumberHelper.Normalize(pageSize, 1, 20);

        var filter = filterMapper.Map(filterModel);
        var filteredEvents = filter.Filter(query);
        var events = new PagedList<Event>(filteredEvents, pageNumber, pageSize);

        return events;
    }
}