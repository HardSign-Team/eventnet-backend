namespace Eventnet.DataAccess;

public class PhotosEntity
{
    public string pathToPhotos { get; set; }
    public Guid Id { get; set; }

    public PhotosEntity(string pathToPhotos, Guid id)
    {
        this.pathToPhotos = pathToPhotos;
        Id = id;
    }
}