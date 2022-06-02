using Eventnet.Api.Models.Photos;
using Eventnet.DataAccess;
using Eventnet.Infrastructure.PhotoServices;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Api.Services.Photo;

public class PhotoService : IPhotoService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IPhotoStorageService photoStorageService;

    public PhotoService(ApplicationDbContext dbContext, IPhotoStorageService photoStorageService)
    {
        this.dbContext = dbContext;
        this.photoStorageService = photoStorageService;
    }

    public async Task<List<PhotoViewModel>> GetTitlePhotos(string basePath, Guid[] eventIds)
    {
        return await dbContext.Photos
            .GroupBy(x => x.EventId)
            .Where(x => eventIds.Contains(x.Key))
            .Select(x => new { EventId = x.Key, PhotoId = x.Select(y => y.Id).FirstOrDefault() })
            .Select(x => new PhotoViewModel(x.EventId, x.PhotoId, GetPhotoPath(basePath, x.PhotoId)))
            .ToListAsync();
    }

    public async Task<List<PhotoViewModel>> GetPhotosViewModels(string basePath, Guid eventId)
    {
        var ids = await dbContext.Photos
            .Where(x => x.EventId == eventId)
            .Select(x => x.Id)
            .ToListAsync();
        return ids.Select(photoId => new PhotoViewModel(eventId, photoId, GetPhotoPath(basePath, photoId))).ToList();
    }

    private string GetPhotoPath(string basePath, Guid photoId) => basePath + photoStorageService.GetPhotoPath(photoId);
}