using Eventnet.Domain;

namespace Eventnet.Infrastructure.PhotoServices;

public interface IPhotoToStorageSaveService
{
    void Save(Photo photo, Guid photoId);
}