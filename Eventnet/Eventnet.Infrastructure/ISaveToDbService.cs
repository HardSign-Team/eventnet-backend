using System.Drawing;
using Eventnet.Models;

namespace Eventnet.Infrastructure;

public interface ISaveToDbService
{
    void SaveImages(List<Image> images, Guid id);
    void SaveEvent(Event eventForSave);
}