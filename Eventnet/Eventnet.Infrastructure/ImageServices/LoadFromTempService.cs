using System.Drawing;

namespace Eventnet.Infrastructure.ImageServices;

public class LoadFromTempService : ILoadFromTempService
{
    public List<Image> LoadImages(string path)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Can't load images. No such directory {path}");
        return Directory
            .GetFiles(path)
            .Select(Image.FromFile)
            .ToList();
    }
}