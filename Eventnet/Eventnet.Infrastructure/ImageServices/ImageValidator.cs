using System.Drawing;
using Eventnet.Infrastructure.ImageServices;

namespace Eventnet.Infrastructure.ImageServices;

public class ImageValidator : IImageValidator
{
    public bool Validate(List<Image> images, out string exception)
    {
        // надо придумать что проверять на бэкенде.
        exception = "";
        return true;
    }
}