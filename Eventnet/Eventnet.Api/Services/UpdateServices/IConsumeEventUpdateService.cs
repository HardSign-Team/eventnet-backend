namespace Eventnet.Api.Services.UpdateServices;

public interface IConsumeEventUpdateService : IDisposable
{
    void ConsumeAndHandle();
}