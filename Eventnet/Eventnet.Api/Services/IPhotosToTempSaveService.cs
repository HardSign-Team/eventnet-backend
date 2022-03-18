namespace Eventnet.Services;

public interface IPhotosToTempSaveService
{
    string SaveToTemp(Guid id, IFormFile[] photos);
}