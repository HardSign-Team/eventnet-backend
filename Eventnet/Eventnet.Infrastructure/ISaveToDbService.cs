using System.Drawing;
using Eventnet.Models;

namespace Eventnet.Infrastructure;

public interface ISaveToDbService
{
    Task SavePhotos(List<Image> photos, Guid eventId);
    Task SaveEvent(Event eventForSave);
}