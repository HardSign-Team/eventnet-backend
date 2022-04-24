using System.Text;
using System.Text.Json;
using Eventnet.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Eventnet.Api.Services.SaveServices;

public class RabbitMqConsumeEventService : IConsumeEventService
{
    private readonly IModel channel;
    private readonly IConnection connection;
    private readonly string queue;

    public RabbitMqConsumeEventService(RabbitMqConfig config)
    {
        queue = config.Queue;
        var connectionFactory = new ConnectionFactory { HostName = config.HostName };
        connection = connectionFactory.CreateConnection();
        channel = connection.CreateModel();
        channel.QueueDeclare(queue, true, false, false);
    }

    public void ConsumeAndHandle(Action<RabbitMqMessage> handle)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<RabbitMqMessage>(content);
            handle(message!);
            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queue, false, consumer);
    }

    public void Dispose()
    {
        channel.Close();
        connection.Close();
    }
}