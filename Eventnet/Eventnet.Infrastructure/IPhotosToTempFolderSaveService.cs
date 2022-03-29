namespace Eventnet.Infrastructure;

public interface IPhotosToTempFolderSaveService
{ 
    string SaveToTempFolder(Guid id, Stream[] streams);
}