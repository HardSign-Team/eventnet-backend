namespace Eventnet.DataAccess.Entities;

public class LocationEntity
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public LocationEntity()
    {
    }

    public LocationEntity(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}