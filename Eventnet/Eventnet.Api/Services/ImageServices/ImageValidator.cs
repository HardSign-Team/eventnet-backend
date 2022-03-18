using System.Drawing;

namespace Eventnet.Services.ImageServices;

public class ImageValidator : IImageValidator
{
    public bool Validate(List<Image> images, out string exception)
    {
        // надо придумать что проверять на бэкенде.
        exception = "";
        return true;
    }
}