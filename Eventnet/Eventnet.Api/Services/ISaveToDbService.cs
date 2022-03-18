using System.Drawing;

namespace Eventnet.Services;

public interface ISaveToDbService
{
    void SaveImages(List<Image> images, Guid id);
    void SaveEvent(RabbitMqMessage message);
}