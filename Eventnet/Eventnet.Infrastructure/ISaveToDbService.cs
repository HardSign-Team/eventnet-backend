using Eventnet.Domain;
using Eventnet.Domain.Events;

namespace Eventnet.Infrastructure;

public interface ISaveToDbService
{
    Task SavePhotos(List<Photo> photos, Guid eventId);
    Task SaveEvent(Event eventForSave);
}