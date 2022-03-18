namespace Eventnet.DataAccess;

public class PhotosEntity
{
    private readonly string pathToPhotos;
    public Guid Id { get; }

    public PhotosEntity(string pathToPhotos, Guid id)
    {
        this.pathToPhotos = pathToPhotos;
        Id = id;
    }
}