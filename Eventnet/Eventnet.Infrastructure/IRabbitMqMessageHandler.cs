namespace Eventnet.Infrastructure;

public interface IRabbitMqMessageHandler
{
    Task HandleAsync(RabbitMqMessage rabbitMqMessage);
}