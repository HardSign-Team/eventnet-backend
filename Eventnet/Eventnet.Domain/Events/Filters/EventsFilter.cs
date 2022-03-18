using Eventnet.DataAccess;
using Eventnet.Domain.Events.Filters.EventFilters;

namespace Eventnet.Domain.Events.Filters;

public class EventsFilter : IEventsFilter
{
    private readonly Func<EventEntity, bool> filter;

    public EventsFilter(IEnumerable<IEventFilter> filters)
    {
        filter = CreateComposedFilter(filters);
    }

    public IEnumerable<EventEntity> Filter(IEnumerable<EventEntity> query)
    {
        return query.Where(x => filter(x));
    }

    private static Func<EventEntity, bool> CreateComposedFilter(IEnumerable<IEventFilter> filters)
    {
        var filter = (EventEntity ev) => true;
        foreach (var f in filters)
        {
            var copy = filter;
            filter = (ev) => copy(ev) && f.Filter(ev);
        }

        return filter;
    }
}