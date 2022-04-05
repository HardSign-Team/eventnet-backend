using System.Drawing;

namespace Eventnet.Infrastructure.ImageServices;

public interface IPhotoToStorageSaveService
{
    void Save(Image photo, Guid photoId);
}