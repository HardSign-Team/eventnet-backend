using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain;

namespace Eventnet.Infrastructure.PhotoServices;

public class PhotosDbService : IPhotosDbService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IPhotoStorageService storageService;
    public PhotosDbService(
        IPhotoStorageService storageService,
        ApplicationDbContext dbContext)
    {
        this.storageService = storageService;
        this.dbContext = dbContext;
    }
    
    public async Task SavePhotosAsync(List<Photo> photos, Guid eventId)
    {
        foreach (var photo in photos)
        {
            var photoId = Guid.NewGuid();
            storageService.Save(photo, photoId);
            var photoEntity = new PhotoEntity(photoId, eventId);
            await SavePhotoToDbAsync(photoEntity);
        }
    }

    public async Task DeletePhotosAsync(Guid[] guidsToDelete)
    {
        var photosToDelete = dbContext.Photos
            .Where(photo => guidsToDelete.Contains(photo.EventId))
            .ToList();
        dbContext.Photos.RemoveRange(photosToDelete);
        await dbContext.SaveChangesAsync();
        foreach (var guid in guidsToDelete)
        {
            storageService.Delete(guid);
        }
    }

    private async Task SavePhotoToDbAsync(PhotoEntity photoEntity)
    {
        dbContext.Photos.Add(photoEntity);
        await dbContext.SaveChangesAsync();
    }
}