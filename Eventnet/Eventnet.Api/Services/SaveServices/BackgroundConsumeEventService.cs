using Eventnet.Infrastructure;

namespace Eventnet.Services.SaveServices;

public class BackgroundConsumeEventService : BackgroundService
{
    private readonly IRabbitMqMessageHandler rabbitMqMessageHandler;
    private readonly IConsumeEventService consumeEventService;

    public BackgroundConsumeEventService(IRabbitMqMessageHandler rabbitMqMessageHandler, IConsumeEventService consumeEventService)
    {
        this.rabbitMqMessageHandler = rabbitMqMessageHandler;
        this.consumeEventService = consumeEventService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        consumeEventService.ConsumeAndHandle(rabbitMqMessageHandler.Handle);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        consumeEventService.Dispose();
        base.Dispose();
    }
}