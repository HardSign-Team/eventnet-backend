using Eventnet.Api.Models;
using Eventnet.Domain.Events.Filters;

namespace Eventnet.Api.Services;

public interface IEventFilterMapper
{
    EventsFilter Map(EventsFilterModel eventsFilterModel);
}