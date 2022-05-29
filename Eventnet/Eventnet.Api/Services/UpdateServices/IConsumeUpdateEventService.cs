namespace Eventnet.Api.Services.UpdateServices;

public interface IConsumeUpdateEventService : IDisposable
{
    void ConsumeAndHandle();
}