using System.Text;
using RabbitMQ.Client;

namespace Eventnet.Api.Services.SaveServices;

public abstract class EventPublisher : IDisposable
{
    private readonly IModel channel;
    private readonly IConnection connection;
    private readonly string queue;

    protected EventPublisher(string queueName, string hostName, int port)
    {
        queue = queueName;
        var factory = new ConnectionFactory { HostName = hostName, Port = port };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.QueueDeclare(queue, true, false, false);
    }

    public void Dispose()
    {
        channel.Dispose();
        connection.Dispose();
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
}