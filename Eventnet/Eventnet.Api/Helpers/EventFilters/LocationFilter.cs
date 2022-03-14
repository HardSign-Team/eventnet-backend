using Eventnet.DataAccess;
using Eventnet.Models;
using GeoCoordinatePortable;

namespace Eventnet.Helpers.EventFilters;

public class LocationFilter : IEventFilter
{
    private readonly GeoCoordinate coordinate;
    private readonly double radius;

    public LocationFilter(LocationFilterModel locationFilterModel)
    {
        var ((latitude, longitude), r) = locationFilterModel;
        coordinate = new GeoCoordinate(latitude, longitude);
        radius = r;
    }

    public bool Filter(EventEntity entity)
    {
        var otherCoordinate = new GeoCoordinate(entity.Location.Latitude, entity.Location.Longitude);
        var distanceTo = coordinate.GetDistanceTo(otherCoordinate);
        return distanceTo < radius;
    }
}