using System;
using Eventnet.Infrastructure;
using Eventnet.Services.SaveServices;

namespace Eventnet.Api.IntegrationTests.Mocks;

public class ConsumeEventMock : IConsumeEventService
{
    public void Dispose()
    {
        
    }

    public void ConsumeAndHandle(Action<RabbitMqMessage> handle)
    {
        
    }
}