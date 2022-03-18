using System.Drawing;
using System.Drawing.Imaging;

namespace Eventnet.Services.ImageServices;

public class ImageToDbPreparer : IImageToDbPreparer
{
    public string Save(List<Image> images, Guid id)
    {
        var dirToSave = "images"; // позже вынести в конфиг, когда он появится
        if (!Directory.Exists(dirToSave))
            Directory.CreateDirectory(dirToSave);
        var dir = Path.Combine(dirToSave, id.ToString());
        Directory.CreateDirectory(dir);
        for (var i = 0; i < images.Count; i++)
        {
            var path = Path.Combine(dir, i + ".jpeg");
            var image = images[i];
            image.Save(path, ImageFormat.Jpeg);
        }

        return dir;
    }
}