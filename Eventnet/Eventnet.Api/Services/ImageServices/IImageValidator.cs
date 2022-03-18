using System.Drawing;

namespace Eventnet.Services.ImageServices;

public interface IImageValidator
{
    bool Validate(List<Image> images, out string exception);
}