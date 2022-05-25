namespace Eventnet.Api.Services.SaveServices;

public interface IConsumeSaveEventService : IDisposable
{
    void ConsumeAndHandle();
}