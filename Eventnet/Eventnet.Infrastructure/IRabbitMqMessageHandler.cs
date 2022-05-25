namespace Eventnet.Infrastructure;

public interface IRabbitMqMessageHandler
{
    Task HandleAsync(RabbitMqSaveMessage rabbitMqSaveMessage);
}