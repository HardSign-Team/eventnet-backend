namespace Eventnet.Api.Services.UpdateServices;

public class BackgroundConsumeUpdateEventService : BackgroundService
{
    private readonly IConsumeEventUpdateService consumeEventUpdateService;

    public BackgroundConsumeUpdateEventService(IConsumeEventUpdateService consumeEventUpdateService)
    {
        this.consumeEventUpdateService = consumeEventUpdateService;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        consumeEventUpdateService.ConsumeAndHandle();
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        consumeEventUpdateService.Dispose();
        base.Dispose();
    }
}