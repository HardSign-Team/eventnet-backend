using Eventnet.Api.Helpers.EventFilterFactories;
using Eventnet.Api.Models;
using Eventnet.Domain.Events.Filters;

namespace Eventnet.Api.Services;

public class EventFilterMapper : IEventFilterMapper
{
    private readonly IEnumerable<IEventFilterFactory> factories;

    public EventFilterMapper(IEnumerable<IEventFilterFactory> factories) => this.factories = factories;

    public EventsFilter Map(EventsFilterModel eventsFilterModel)
    {
        var filters = factories
            .Where(x => x.ShouldCreate(eventsFilterModel))
            .Select(x => x.Create(eventsFilterModel));
        return new EventsFilter(filters);
    }
}