using Eventnet.Domain;

namespace Eventnet.Infrastructure.PhotoServices;

public class PhotoStorageService : IPhotoStorageService
{
    private readonly string dirToSave;

    public PhotoStorageService(PhotoStorageConfig config)
    {
        if (!Directory.Exists(config.Path))
            throw new DirectoryNotFoundException($"Directory {config.Path} not found");
        dirToSave = config.Path;
    }

    public void Save(Photo photo, Guid photoId)
    {
        photo.Save(GetPhotoPath(photoId));
    }

    public string GetPhotoPath(Guid photoId) => Path.Combine(dirToSave, photoId.ToString());
}