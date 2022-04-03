using GeoCoordinatePortable;

namespace Eventnet.Domain.Events;

public record Location(double Latitude, double Longitude)
{
    public double DistanceTo(Location other)
    {
        var coordinate = new GeoCoordinate(Latitude, Longitude);
        var (latitude, longitude) = other;
        var otherCoordinate = new GeoCoordinate(latitude, longitude);
        var distanceTo = coordinate.GetDistanceTo(otherCoordinate);
        return distanceTo;
    }
}