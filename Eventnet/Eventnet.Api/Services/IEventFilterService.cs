using Eventnet.DataAccess;
using Eventnet.Models;

namespace Eventnet.Services;

public interface IEventFilterService
{
    IEnumerable<EventEntity> FilterAsync(IQueryable<EventEntity> query, FilterEventsModel filterModel);
}