using Eventnet.Helpers.EventFilters;
using Eventnet.Models;

namespace Eventnet.Helpers.EventFilterFactories;

public abstract class BaseFilterFactory<T> : IEventFilterFactory where T : class
{
    public bool ShouldCreate(FilterEventsModel filterModel) => GetProperty(filterModel) is not null;

    public IEventFilter Create(FilterEventsModel filterModel) =>
        GetProperty(filterModel) is { } property ? Create(property) : NullFilter.Instance;

    protected abstract T? GetProperty(FilterEventsModel model);
    protected abstract IEventFilter Create(T property);
}