namespace Eventnet.Infrastructure;

public class PhotosToTempFolderSaveService : IPhotosToTempFolderSaveService
{
    public string SaveToTempFolder(Guid id, Stream[] streams)
    {
        var pathToPhotos = "temp"; // TODO: вынести в кофниг, когда он повяится
        if (!Directory.Exists(pathToPhotos))
            Directory.CreateDirectory(pathToPhotos);
        var eventPhotosPath = Path.Combine(pathToPhotos, id.ToString());
        Directory.CreateDirectory(eventPhotosPath);
        for (var i = 0; i < streams.Length; i++)
        {
            var file = streams[i];
            if (file.Length <= 0)
                continue;
            var filePath = Path.Combine(eventPhotosPath, i.ToString());
            using Stream fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);
        }

        return eventPhotosPath;
    }
}