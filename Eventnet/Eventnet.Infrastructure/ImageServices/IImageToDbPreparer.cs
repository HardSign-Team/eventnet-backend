using System.Drawing;

namespace Eventnet.Infrastructure.ImageServices;

public interface IImageToDbPreparer
{
    string Save(List<Image> images, Guid id);
}