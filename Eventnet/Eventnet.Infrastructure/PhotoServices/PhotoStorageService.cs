using Eventnet.Domain;

namespace Eventnet.Infrastructure.PhotoServices;

public class PhotoStorageService : IPhotoStorageService
{
    private readonly string dirToSave;

    public PhotoStorageService(PhotoStorageConfig config)
    {
        Directory.CreateDirectory(config.Path);
        dirToSave = config.Path;
    }

    public void Save(Photo photo, Guid photoId)
    {
        photo.Save(GetPhotoPath(photoId));
    }

    public void Delete(Guid photoId)
    {
        var path = GetPhotoPath(photoId) + ".jpeg";
        if (File.Exists(path))
            File.Delete(path);
    }

    public string GetPhotoPath(Guid photoId) => Path.Combine(dirToSave, photoId.ToString());
}