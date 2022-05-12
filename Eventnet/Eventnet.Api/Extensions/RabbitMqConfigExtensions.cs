using Eventnet.Api.Config;

namespace Eventnet.Api.Extensions;

public static class RabbitMqConfigExtensions
{
    public static bool IsPhotosSizeLessThanRecommended(this RabbitMqConfig config, IEnumerable<IFormFile> photos)
    {
        var photosSize = photos.Sum(photo => photo.Length);
        return photosSize >= config.RecommendedMessageSizeInBytes;
    }

    public static int RecommendedMessageSizeInMb(this RabbitMqConfig config) => 
        config.RecommendedMessageSizeInBytes / (1024 * 1024);
}