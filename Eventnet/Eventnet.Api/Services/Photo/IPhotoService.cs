using Eventnet.Api.Models.Photos;

namespace Eventnet.Api.Services.Photo;

public interface IPhotoService
{
    Task<List<PhotoViewModel>> GetTitlePhotos(string basePath, Guid[] modelIds);
    Task<List<PhotoViewModel>> GetPhotosViewModels(string basePath, Guid eventId);
}