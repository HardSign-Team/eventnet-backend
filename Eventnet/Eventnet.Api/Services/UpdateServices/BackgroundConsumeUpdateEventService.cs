namespace Eventnet.Api.Services.UpdateServices;

public class BackgroundConsumeUpdateEventService : BackgroundService
{
    private readonly IConsumeUpdateEventService consumeUpdateEventService;

    public BackgroundConsumeUpdateEventService(IConsumeUpdateEventService consumeUpdateEventService)
    {
        this.consumeUpdateEventService = consumeUpdateEventService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        consumeUpdateEventService.ConsumeAndHandle();
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        consumeUpdateEventService.Dispose();
        base.Dispose();
    }
}