using Eventnet.DataAccess;

namespace Eventnet.Helpers.EventFilters;

public class NullFilter : IEventFilter
{
    public static readonly IEventFilter Instance = new NullFilter();
    public bool Filter(EventEntity entity) => true;
}