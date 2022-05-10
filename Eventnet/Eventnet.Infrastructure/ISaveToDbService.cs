using Eventnet.Domain;
using Eventnet.Domain.Events;

namespace Eventnet.Infrastructure;

public interface ISaveToDbService
{
    Task SavePhotosAsync(List<Photo> photos, Guid eventId);
    Task SaveEventAsync(Event eventForSave);
}