namespace Eventnet.Infrastructure;

public interface IPhotosToTempSaveService
{ 
    string SaveToTemp(Guid id, Stream[] streams);
}