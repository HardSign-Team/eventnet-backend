namespace Eventnet.Services.ImageServices;

public interface IPhotosToTempSaveService
{
    string SaveToTemp(Guid id, IFormFile[] photos);
}