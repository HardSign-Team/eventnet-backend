using System.Drawing;

namespace Eventnet.Services.ImageServices;

public interface IImageToDbPreparer
{
    string Save(List<Image> images, Guid id);
}