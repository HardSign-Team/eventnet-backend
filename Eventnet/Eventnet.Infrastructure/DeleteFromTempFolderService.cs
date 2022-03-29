namespace Eventnet.Infrastructure;

public class DeleteFromTempFolderService : IDeleteFromTempFolderService
{
    public void Delete(string path)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Cant' delete directory. Directory {path} doesn't exist");
        Directory.Delete(path, true);
    }
}