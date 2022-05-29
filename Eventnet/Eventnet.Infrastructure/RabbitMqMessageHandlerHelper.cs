using Eventnet.Domain;

namespace Eventnet.Infrastructure;

public class RabbitMqMessageHandlerHelper
{
    public List<Photo> GetPhotos(IEnumerable<RabbitMqPhoto> rabbitMqPhotos) =>
        rabbitMqPhotos
            .Select(rabbitMqPhoto => new Photo(rabbitMqPhoto.PhotoInBytes, rabbitMqPhoto.ContentType))
            .ToList();
}