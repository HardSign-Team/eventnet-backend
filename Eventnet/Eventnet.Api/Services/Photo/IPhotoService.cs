using Eventnet.Api.Models.Photos;

namespace Eventnet.Api.Services.Photo;

public interface IPhotoService
{
    Task<List<PhotoViewModel>> GetTitlePhotos(string basePath, Guid[] modelIds);
    Task<List<string>> GetPhotoUrls(string basePath, Guid eventId);
}