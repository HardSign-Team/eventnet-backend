using Eventnet.Api.Models;
using Eventnet.Domain.Events.Filters.EventFilters;

namespace Eventnet.Api.Helpers.EventFilterFactories;

public abstract class BaseFilterFactory<T> : IEventFilterFactory where T : class
{
    public bool ShouldCreate(EventsFilterModel eventsFilterModel) => GetProperty(eventsFilterModel) is not null;

    public IEventFilter Create(EventsFilterModel eventsFilterModel) =>
        GetProperty(eventsFilterModel) is { } property ? Create(property) : NullFilter.Instance;

    protected abstract T? GetProperty(EventsFilterModel model);
    protected abstract IEventFilter Create(T property);
}