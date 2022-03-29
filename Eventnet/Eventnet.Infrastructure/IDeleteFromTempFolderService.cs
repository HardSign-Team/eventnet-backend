namespace Eventnet.Infrastructure;

public interface IDeleteFromTempFolderService
{
    void Delete(string path);
}