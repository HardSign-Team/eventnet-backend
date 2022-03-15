using Eventnet.DataAccess;

namespace Eventnet.Domain.Events.Filters;

public interface IEventsFilter
{
    IEnumerable<EventEntity> Filter(IEnumerable<EventEntity> query);
}