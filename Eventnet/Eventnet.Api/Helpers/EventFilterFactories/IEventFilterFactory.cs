using Eventnet.Api.Models;
using Eventnet.Domain.Events.Filters.EventFilters;

namespace Eventnet.Api.Helpers.EventFilterFactories;

public interface IEventFilterFactory
{
    bool ShouldCreate(EventsFilterModel eventsFilterModel);

    IEventFilter Create(EventsFilterModel eventsFilterModel);
}