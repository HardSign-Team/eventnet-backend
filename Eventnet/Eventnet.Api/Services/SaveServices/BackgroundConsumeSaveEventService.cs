namespace Eventnet.Api.Services.SaveServices;

public class BackgroundConsumeSaveEventService : BackgroundService
{
    private readonly IConsumeSaveEventService consumeSaveEventService;

    public BackgroundConsumeSaveEventService(IConsumeSaveEventService consumeSaveEventService)
    {
        this.consumeSaveEventService = consumeSaveEventService;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        consumeSaveEventService.ConsumeAndHandle();
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        consumeSaveEventService.Dispose();
        base.Dispose();
    }
}