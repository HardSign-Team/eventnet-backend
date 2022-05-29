using Eventnet.Domain;

namespace Eventnet.Infrastructure.PhotoServices;

public interface IPhotosDbService
{
    Task SavePhotosAsync(List<Photo> photos, Guid eventId);
    Task DeletePhotosAsync(Guid[] guidsToDelete);
}