namespace Eventnet.Services.ImageServices;

public class PhotosToTempSaveService : IPhotosToTempSaveService
{
    public string SaveToTemp(Guid id, IFormFile[] photos)
    {
        var pathToPhotos = "temp"; // TODO: вынести в кофниг, когда он повяится
        if (!Directory.Exists(pathToPhotos))
            Directory.CreateDirectory(pathToPhotos);
        var eventPhotosPath = Path.Combine(pathToPhotos, id.ToString());
        Directory.CreateDirectory(eventPhotosPath);
        foreach (var file in photos)
        {
            if (file.Length <= 0) 
                continue;
            var filePath = Path.Combine(eventPhotosPath, file.FileName); 
            using Stream fileStream = new FileStream(filePath, FileMode.Create); 
            file.CopyTo(fileStream);
        }

        return eventPhotosPath;
    }
}