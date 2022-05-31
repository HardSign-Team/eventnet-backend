using Eventnet.Domain;

namespace Eventnet.Infrastructure.PhotoServices;

public class PhotoStorageService : IPhotoStorageService
{
    private const string DefaultAvatarJpeg = "default-avatar.jpeg";
    private readonly string dirToSave;

    public PhotoStorageService(PhotoStorageConfig config)
    {
        Directory.CreateDirectory(config.Path);
        dirToSave = config.Path;
    }

    public void Save(Photo photo, Guid photoId)
    {
        photo.Save(Path.Combine(dirToSave, $"{photoId}.{photo.Extension}"));
    }

    public void Delete(Guid photoId)
    {
        var path = FindPathOfImage(photoId);
        if (path != null)
            File.Delete(path);
    }

    public string GetPhotoPath(Guid photoId) => FindPathOfImage(photoId) ?? DefaultAvatarJpeg;

    private string? FindPathOfImage(Guid photoId)
        => Directory.GetFiles(dirToSave, $"{photoId}.*").FirstOrDefault();
}