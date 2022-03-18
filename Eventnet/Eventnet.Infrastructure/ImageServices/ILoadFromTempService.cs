using System.Drawing;

namespace Eventnet.Infrastructure.ImageServices;

public interface ILoadFromTempService
{
    List<Image> LoadImages(string path);
}