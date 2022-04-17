using Eventnet.Domain.Events.Filters.EventFilters;

namespace Eventnet.Domain.Events.Filters;

public class EventsFilter : IEventsFilter
{
    private readonly Func<Event, bool> filter;

    public EventsFilter(IEnumerable<IEventFilter> filters)
    {
        filter = CreateComposedFilter(filters);
    }

    public IEnumerable<Event> Filter(IEnumerable<Event> query)
    {
        return query.Where(x => filter(x));
    }

    private static Func<Event, bool> CreateComposedFilter(IEnumerable<IEventFilter> filters)
    {
        var filter = (Event ev) => true;
        foreach (var f in filters)
        {
            var copy = filter;
            filter = ev => copy(ev) && f.Filter(ev);
        }

        return filter;
    }
}