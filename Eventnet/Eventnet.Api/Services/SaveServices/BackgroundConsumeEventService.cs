namespace Eventnet.Api.Services.SaveServices;

public class BackgroundConsumeEventService : BackgroundService
{
    private readonly IConsumeEventService consumeEventService;

    public BackgroundConsumeEventService(IConsumeEventService consumeEventService)
    {
        this.consumeEventService = consumeEventService;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        consumeEventService.ConsumeAndHandle();
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        consumeEventService.Dispose();
        base.Dispose();
    }
}