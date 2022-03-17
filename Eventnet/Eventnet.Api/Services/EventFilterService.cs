using Eventnet.DataAccess;
using Eventnet.Helpers.EventFilterFactories;
using Eventnet.Helpers.EventFilters;
using Eventnet.Models;

namespace Eventnet.Services;

public class EventFilterService : IEventFilterService
{
    private readonly IEnumerable<IEventFilterFactory> factories;

    public EventFilterService(IEnumerable<IEventFilterFactory> factories)
    {
        this.factories = factories;
    }

    public IEnumerable<EventEntity> Filter(IEnumerable<EventEntity> query, FilterEventsModel filterModel)
    {
        var filters = factories
            .Where(x => x.ShouldCreate(filterModel))
            .Select(x => x.Create(filterModel));
        var filter = CreateComposedFilter(filters);
        return query.Where(x => filter(x));
    }

    private Func<EventEntity, bool> CreateComposedFilter(IEnumerable<IEventFilter> filters)
    {
        var filter = (EventEntity ev) => true;
        foreach (var f in filters)
        {
            var copy = filter;
            filter = ev => copy(ev) && f.Filter(ev);
        }

        return filter;
    }
}