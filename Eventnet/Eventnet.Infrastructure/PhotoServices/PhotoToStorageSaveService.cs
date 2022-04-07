using System.Drawing;
using System.Drawing.Imaging;

namespace Eventnet.Infrastructure.PhotoServices;

public class PhotoToStorageSaveService : IPhotoToStorageSaveService
{
    private readonly string dirToSave;

    public PhotoToStorageSaveService(PhotoStorageConfig config)
    {
        dirToSave = config.Path;
        if (!Directory.Exists(dirToSave))
            Directory.CreateDirectory(dirToSave);
    }

    public void Save(Image photo, Guid photoId)
    {
        var path = Path.Combine(dirToSave, photoId + ".jpeg");
        photo.Save(path, ImageFormat.Jpeg);
    }
}