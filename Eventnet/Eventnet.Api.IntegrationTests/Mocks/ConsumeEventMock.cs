using System;
using System.Threading.Tasks;
using Eventnet.Api.Services.SaveServices;
using Eventnet.Infrastructure;

namespace Eventnet.Api.IntegrationTests.Mocks;

public class ConsumeEventMock : IConsumeEventService
{
    public void Dispose()
    {
    }

    public void ConsumeAndHandle(Func<RabbitMqMessage, Task> handle)
    {
    }
}