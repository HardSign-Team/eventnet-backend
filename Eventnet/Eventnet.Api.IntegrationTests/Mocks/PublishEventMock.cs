using System.Threading.Tasks;
using Eventnet.Api.Services.SaveServices;

namespace Eventnet.Api.IntegrationTests.Mocks;

public class PublishEventMock : IPublishEventService
{
    public Task PublishAsync(string message) => Task.CompletedTask;

    public void Dispose()
    {
    }
}