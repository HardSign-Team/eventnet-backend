using Eventnet.Domain.Events;

namespace Eventnet.Api.Services.UpdateServices;

public interface IUpdateEventService
{
    Task SendEventForUpdate(EventInfo eventForUpdate);
    Task SendPhotosForUpdate(Guid eventId, IFormFile[] newPhotos, Guid[] idToDelete);
}