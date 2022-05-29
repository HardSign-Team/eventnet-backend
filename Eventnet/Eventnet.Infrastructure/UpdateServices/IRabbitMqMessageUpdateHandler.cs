namespace Eventnet.Infrastructure.UpdateServices;

public interface IRabbitMqMessageUpdateHandler
{
    Task UpdateAsync(RabbitMqUpdateMessage rabbitMqMessageSave);
}