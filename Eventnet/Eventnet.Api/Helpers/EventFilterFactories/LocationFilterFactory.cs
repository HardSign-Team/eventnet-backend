using Eventnet.Api.Models.Filtering;
using Eventnet.Domain.Events.Filters.EventFilters;
using GeoCoordinatePortable;

namespace Eventnet.Api.Helpers.EventFilterFactories;

public class LocationFilterFactory : BaseFilterFactory<LocationFilterModel>
{
    protected override LocationFilterModel? GetProperty(EventsFilterModel model) => model.RadiusLocation;

    protected override IEventFilter Create(LocationFilterModel property)
    {
        var ((latitude, longitude), radius) = property;
        return new LocationFilter(new GeoCoordinate(latitude, longitude), radius);
    }
}