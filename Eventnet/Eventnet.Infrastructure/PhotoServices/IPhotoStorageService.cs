using Eventnet.Domain;

namespace Eventnet.Infrastructure.PhotoServices;

public interface IPhotoStorageService
{
    void Save(Photo photo, Guid photoId);
    string GetPhotoPath(Guid arg);
    void Delete(Guid photoId);
}