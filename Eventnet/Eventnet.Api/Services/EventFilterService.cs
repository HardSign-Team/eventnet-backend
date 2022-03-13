using Eventnet.DataAccess;
using Eventnet.Models;

namespace Eventnet.Services;

public class EventFilterService : IEventFilterService
{
    public IEnumerable<EventEntity> FilterAsync(IQueryable<EventEntity> query, FilterEventsModel filterModel)
    {
        // TODO need more complicated solution. Later.
        var (location, radius) = filterModel;
        return query
            .AsEnumerable()
            .Where(x => location.DistanceTo(ToLocation(x.Location)) <= radius);
    }

    private static Location ToLocation(LocationEntity locationEntity) =>
        new(locationEntity.Latitude, locationEntity.Longitude);
}