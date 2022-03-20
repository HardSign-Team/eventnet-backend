using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;

namespace Eventnet.Domain.Events.Filters.EventFilters;

public interface IEventFilter : IFilter<EventEntity>
{
}