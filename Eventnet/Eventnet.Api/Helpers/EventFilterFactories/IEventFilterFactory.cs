using Eventnet.Domain.Events.Filters.EventFilters;
using Eventnet.Models;

namespace Eventnet.Helpers.EventFilterFactories;

public interface IEventFilterFactory
{
    bool ShouldCreate(EventsFilterModel eventsFilterModel);

    IEventFilter Create(EventsFilterModel eventsFilterModel);
}