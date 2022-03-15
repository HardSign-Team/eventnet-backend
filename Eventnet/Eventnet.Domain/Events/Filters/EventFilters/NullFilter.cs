using Eventnet.DataAccess;

namespace Eventnet.Domain.Events.Filters.EventFilters;

public class NullFilter : IEventFilter
{
    public static readonly IEventFilter Instance = new NullFilter();
    public bool Filter(EventEntity entity) => true;
}