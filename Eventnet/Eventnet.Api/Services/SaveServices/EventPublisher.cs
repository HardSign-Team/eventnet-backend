using System.Text;
using RabbitMQ.Client;

namespace Eventnet.Api.Services.SaveServices;

public abstract class EventPublisher : IDisposable
{
    private readonly IModel channel;
    private readonly string queue;
    private readonly IConnection connection;
    
    protected EventPublisher(string queueName, string hostName, int port)
    {
        queue = queueName;
        var factory = new ConnectionFactory { HostName = hostName, Port = port };
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
            channel.BasicPublish("", queue, null, Encoding.UTF8.GetBytes(message));
        });
    }

    public void Dispose()
    {
        channel.Dispose();
        connection.Dispose();
    }
}