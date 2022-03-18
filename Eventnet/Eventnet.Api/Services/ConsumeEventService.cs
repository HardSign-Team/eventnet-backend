using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Eventnet.Services;

public class ConsumeEventService : BackgroundService
{
    private readonly IModel channel;
    private readonly IConnection connection;
    private readonly string queue;
    private readonly IRabbitMqMessageHandler rabbitMqMessageHandler;

    public ConsumeEventService(IRabbitMqMessageHandler rabbitMqMessageHandler, RabbitMqConfig config)
    {
        this.rabbitMqMessageHandler = rabbitMqMessageHandler;
        queue = config.Queue;
        var connectionFactory = new ConnectionFactory { HostName = config.HostName };
        connection = connectionFactory.CreateConnection();
        channel = connection.CreateModel();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<RabbitMqMessage>(content);
            rabbitMqMessageHandler.Handle(message);
            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queue, false, consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        channel.Close();
        connection.Close();
        base.Dispose();
    }
}