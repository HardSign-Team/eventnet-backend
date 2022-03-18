using Eventnet.Domain.Events.Filters;
using Eventnet.Models;

namespace Eventnet.Services;

public interface IEventFilterMapper
{
    EventsFilter Map(EventsFilterModel eventsFilterModel);
}