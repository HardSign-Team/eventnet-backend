using Eventnet.Domain;

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

    public void Save(Photo photo, Guid photoId)
    {
        var path = Path.Combine(dirToSave, photoId.ToString());
        photo.Save(path);
    }
}