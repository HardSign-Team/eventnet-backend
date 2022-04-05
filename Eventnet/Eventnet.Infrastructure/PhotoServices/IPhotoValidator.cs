using System.Drawing;

namespace Eventnet.Infrastructure.PhotoServices;

public interface IPhotoValidator
{
    bool Validate(List<Image> photos, out string exception);
}