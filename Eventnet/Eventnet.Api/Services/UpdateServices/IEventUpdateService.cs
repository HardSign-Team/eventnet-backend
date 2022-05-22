using Eventnet.Domain.Events;

namespace Eventnet.Api.Services.UpdateServices;

public interface IEventUpdateService
{
    Task SendEventForUpdate(EventInfo eventForUpdate);
    Task SendPhotosForUpdate(Guid eventId, IFormFile[] newPhotos, Guid[] idToDelete);
}