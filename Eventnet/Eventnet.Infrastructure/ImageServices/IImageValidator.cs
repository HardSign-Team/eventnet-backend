using System.Drawing;

namespace Eventnet.Infrastructure.ImageServices;

public interface IImageValidator
{
    bool Validate(List<Image> images, out string exception);
}