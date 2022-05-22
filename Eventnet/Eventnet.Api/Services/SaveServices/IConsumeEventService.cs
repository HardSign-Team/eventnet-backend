using Eventnet.Infrastructure;

namespace Eventnet.Api.Services.SaveServices;

public interface IConsumeEventService : IDisposable
{
    void ConsumeAndHandle();
}