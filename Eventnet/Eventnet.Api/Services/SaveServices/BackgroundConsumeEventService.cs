using Eventnet.Infrastructure;

namespace Eventnet.Api.Services.SaveServices;

public class BackgroundConsumeEventService : BackgroundService
{
    private readonly IRabbitMqMessageHandler rabbitMqMessageHandler;
    private readonly IConsumeEventService consumeEventService;

    public BackgroundConsumeEventService(
        IRabbitMqMessageHandler rabbitMqMessageHandler,
        IConsumeEventService consumeEventService)
    {
        this.rabbitMqMessageHandler = rabbitMqMessageHandler;
        this.consumeEventService = consumeEventService;
    }

    public override void Dispose()
    {
        consumeEventService.Dispose();
        base.Dispose();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        consumeEventService.ConsumeAndHandle(rabbitMqMessageHandler.HandleAsync);
        return Task.CompletedTask;
    }
}