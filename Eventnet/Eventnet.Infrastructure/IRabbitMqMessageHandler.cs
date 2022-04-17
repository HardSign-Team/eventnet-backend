namespace Eventnet.Infrastructure;

public interface IRabbitMqMessageHandler
{
    void Handle(RabbitMqMessage rabbitMqMessage);
}