using Eventnet.DataAccess;
using Eventnet.Models;

namespace Eventnet.Services;

public interface IEventFilterService
{
    IEnumerable<EventEntity> Filter(IQueryable<EventEntity> query, FilterEventsModel filterModel);
}