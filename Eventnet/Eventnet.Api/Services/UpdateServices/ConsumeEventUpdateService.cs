using System.Text;
using System.Text.Json;
using Eventnet.Api.Config;
using Eventnet.Infrastructure.UpdateServices;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Eventnet.Api.Services.UpdateServices;

public class ConsumeEventUpdateService : IConsumeEventUpdateService
{
    private readonly IRabbitMqMessageUpdateHandler rabbitMqMessageUpdateHandler;
    private readonly IModel channel;
    private readonly IConnection connection;
    private readonly string queue;

    public ConsumeEventUpdateService(RabbitMqConfig config, IRabbitMqMessageUpdateHandler rabbitMqMessageUpdateHandler)
    {
        this.rabbitMqMessageUpdateHandler = rabbitMqMessageUpdateHandler;
        queue = config.QueueEventUpdate;
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
            var message = JsonSerializer.Deserialize<RabbitMqUpdateMessage>(content);
            await rabbitMqMessageUpdateHandler.UpdateAsync(message!);
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