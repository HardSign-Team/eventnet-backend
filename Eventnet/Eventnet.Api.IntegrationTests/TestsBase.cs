using System;
using System.Net.Http;
using Eventnet.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Eventnet.Api.IntegrationTests;

public class TestsBase : AbstractTestBase
{
    protected override WebApplicationFactory<Program> Factory { get; } = new TestWebApplicationFactory<Program>();
}