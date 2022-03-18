using System.Drawing;

namespace Eventnet.Services.ImageServices;

public interface ILoadFromTempService
{ 
    List<Image> LoadImages(string path);
}