namespace Eventnet.Api.Models.Events;


public record PhotosUpdateModel(
    IFormFile[] NewPhotos,
    Guid[] PhotosIdToDelete);