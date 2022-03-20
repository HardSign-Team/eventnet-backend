using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;

namespace Eventnet.Domain.Events.Selectors;

public interface IEventsByNameSelector
{
    IEnumerable<EventEntity> Select(IEnumerable<EventEntity> events, int maxCount);
}