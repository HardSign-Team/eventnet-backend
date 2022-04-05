using System.Drawing;
using System.Drawing.Imaging;

namespace Eventnet.Infrastructure.ImageServices;

public class PhotoToStorageSaveService : IPhotoToStorageSaveService
{
    private const string DirToSave = "storage";
    public PhotoToStorageSaveService()
    {
        if (!Directory.Exists(DirToSave))
            Directory.CreateDirectory(DirToSave);
    }

    public void Save(Image photo, Guid photoId)
    {
        var path = Path.Combine(DirToSave, photoId + ".jpeg");
        photo.Save(path, ImageFormat.Jpeg);
    }
}