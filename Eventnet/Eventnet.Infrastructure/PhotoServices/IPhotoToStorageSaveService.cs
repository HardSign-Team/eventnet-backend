using System.Drawing;

namespace Eventnet.Infrastructure.PhotoServices;

public interface IPhotoToStorageSaveService
{
    void Save(Image photo, Guid photoId);
}