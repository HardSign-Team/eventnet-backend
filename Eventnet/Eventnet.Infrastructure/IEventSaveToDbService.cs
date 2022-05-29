using Eventnet.Domain;
using Eventnet.Domain.Events;

namespace Eventnet.Infrastructure;

public interface IEventSaveToDbService
{
    Task SaveEventAsync(EventInfo info);
    Task UpdateEventAsync(EventInfo eventInfo);
}