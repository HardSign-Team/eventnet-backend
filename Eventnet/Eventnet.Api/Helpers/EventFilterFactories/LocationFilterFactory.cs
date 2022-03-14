using Eventnet.Helpers.EventFilters;
using Eventnet.Models;

namespace Eventnet.Helpers.EventFilterFactories;

public class LocationFilterFactory : BaseFilterFactory<LocationFilterModel>
{
    protected override LocationFilterModel? GetProperty(FilterEventsModel model) => model.RadiusLocation;

    protected override IEventFilter Create(LocationFilterModel property) => new LocationFilter(property);
}