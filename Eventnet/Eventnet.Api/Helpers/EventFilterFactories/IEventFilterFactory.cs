using Eventnet.Helpers.EventFilters;
using Eventnet.Models;

namespace Eventnet.Helpers.EventFilterFactories;

public interface IEventFilterFactory
{
    bool ShouldCreate(FilterEventsModel filterModel);

    IEventFilter Create(FilterEventsModel filterModel);
}