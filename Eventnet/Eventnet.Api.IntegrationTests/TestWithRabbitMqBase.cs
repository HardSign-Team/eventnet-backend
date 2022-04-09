using Microsoft.AspNetCore.Mvc.Testing;

namespace Eventnet.Api.IntegrationTests;

public class TestWithRabbitMqBase : AbstractTestBase
{
    protected override WebApplicationFactory<Program> Factory { get; } = new RabbitMqTestFactory<Program>();
}