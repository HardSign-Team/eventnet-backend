using System.Drawing;

namespace Eventnet.Infrastructure.ImageServices;

public class LoadFromTempService : ILoadFromTempService
{
    public List<Image> LoadImages(string path)
    {
        var images = new List<Image>();
        var filenames = Directory.GetFiles(path);
        foreach (var filename in filenames)
        {
            images.Add(Image.FromFile(filename));
            File.Delete(filename); // возможно, их стоит удалять в другой момент
        }

        return images;
    }
}