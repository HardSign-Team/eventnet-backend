using System.Text;
using System.Text.Json;
using Eventnet.Api.Config;
using Eventnet.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Eventnet.Api.Services.SaveServices;

public class RabbitMqConsumeEventService : IConsumeEventService
{
    private readonly IRabbitMqMessageHandler rabbitMqMessageHandler;
    private readonly IModel channel;
    private readonly IConnection connection;
    private readonly string queue;

    public RabbitMqConsumeEventService(RabbitMqConfig config, IRabbitMqMessageHandler rabbitMqMessageHandler)
    {
        this.rabbitMqMessageHandler = rabbitMqMessageHandler;
        queue = config.Queue;
        var connectionFactory = new ConnectionFactory { HostName = config.HostName, Port = config.Port };
        connection = connectionFactory.CreateConnection();
        channel = connection.CreateModel();
        channel.QueueDeclare(queue, true, false, false);
    }

    public void ConsumeAndHandle()
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (_, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<RabbitMqMessage>(content);
            await rabbitMqMessageHandler.HandleAsync(message!);
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