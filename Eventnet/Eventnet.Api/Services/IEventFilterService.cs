using Eventnet.DataAccess;
using Eventnet.Models;

namespace Eventnet.Services;

public interface IEventFilterService
{
    IEnumerable<EventEntity> Filter(IEnumerable<EventEntity> query, FilterEventsModel filterModel);
}