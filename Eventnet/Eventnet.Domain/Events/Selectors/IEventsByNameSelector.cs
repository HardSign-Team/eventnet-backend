using Eventnet.DataAccess;

namespace Eventnet.Domain.Events.Selectors;

public interface IEventsByNameSelector
{
    IEnumerable<EventEntity> Select(IEnumerable<EventEntity> events, int maxCount);
}