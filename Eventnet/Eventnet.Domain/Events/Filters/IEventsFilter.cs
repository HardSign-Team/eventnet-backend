using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;

namespace Eventnet.Domain.Events.Filters;

public interface IEventsFilter
{
    IEnumerable<EventEntity> Filter(IEnumerable<EventEntity> query);
}