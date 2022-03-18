using Eventnet.DataAccess;
using GeoCoordinatePortable;

namespace Eventnet.Domain.Events.Filters.EventFilters;

public class LocationFilter : IEventFilter
{
    private readonly GeoCoordinate coordinate;
    private readonly double radius;

    public LocationFilter(GeoCoordinate coordinate, double radius)
    {
        this.coordinate = coordinate;
        this.radius = radius;
    }

    public bool Filter(EventEntity entity)
    {
        var otherCoordinate = new GeoCoordinate(entity.Location.Latitude, entity.Location.Longitude);
        var distanceTo = coordinate.GetDistanceTo(otherCoordinate);
        return distanceTo < radius;
    }
}