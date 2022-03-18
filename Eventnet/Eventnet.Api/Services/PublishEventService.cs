using System.Text;
using RabbitMQ.Client;

namespace Eventnet.Services;

public class PublishEventService : IPublishEventService
{
    private readonly IModel channel;
    private readonly string queue;

    public PublishEventService(RabbitMqConfig config)
    {
        queue = config.Queue;
        var factory = new ConnectionFactory { HostName = config.HostName }; 
        var connection = factory.CreateConnection(); 
        channel = connection.CreateModel();
    }

    public async Task SendAsync(string message)
    {
        await Task.Run(() =>
        {
            channel.QueueDeclare(queue, true, false, false);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = false;
            channel.BasicPublish(string.Empty, queue, null, Encoding.UTF8.GetBytes(message));
        });
    }
}