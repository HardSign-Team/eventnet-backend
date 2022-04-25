using Microsoft.AspNetCore.Mvc.Testing;

namespace Eventnet.Api.IntegrationTests;

public class TestsBase : AbstractTestBase
{
    protected override WebApplicationFactory<Program> Factory { get; } = new TestWebApplicationFactory<Program>();
}