using System.Text;
using Eventnet.Api.Config;
using RabbitMQ.Client;

namespace Eventnet.Api.Services.SaveServices;

public class PublishEventService : IPublishEventService
{
    private readonly IModel channel;
    private readonly string queue;
    private readonly IConnection connection;

    public PublishEventService(RabbitMqConfig config)
    {
        queue = config.Queue;
        var factory = new ConnectionFactory { HostName = config.HostName, Port = config.Port };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.QueueDeclare(queue, true, false, false);
    }

    public async Task PublishAsync(string message)
    {
        await Task.Run(() =>
        {
            var properties = channel.CreateBasicProperties();
            properties.Persistent = false;
            channel.BasicPublish(string.Empty, queue, null, Encoding.UTF8.GetBytes(message));
        });
    }

    public void Dispose()
    {
        channel.Dispose();
        connection.Dispose();
    }
}