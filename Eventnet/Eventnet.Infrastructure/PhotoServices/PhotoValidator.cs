using System.Drawing;

namespace Eventnet.Infrastructure.PhotoServices;

public class PhotoValidator : IPhotoValidator
{
    public bool Validate(List<Image> photos, out string exception)
    {
        // надо придумать что проверять на бэкенде.
        exception = "";
        return true;
    }
}