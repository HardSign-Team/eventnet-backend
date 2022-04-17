using Eventnet.Api.Models.Filtering;
using Eventnet.Domain.Events.Filters;

namespace Eventnet.Api.Services.Filters;

public interface IEventFilterMapper
{
    EventsFilter Map(EventsFilterModel eventsFilterModel);
}