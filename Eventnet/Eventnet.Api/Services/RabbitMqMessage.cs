using Eventnet.Models;

namespace Eventnet.Services;

public record RabbitMqMessage(
    Event Event,
    string PathToPhotos);