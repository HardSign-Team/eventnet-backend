namespace Eventnet.Services;

public interface IRabbitMqMessageHandler
{
    void Handle(RabbitMqMessage rabbitMqMessage);
}