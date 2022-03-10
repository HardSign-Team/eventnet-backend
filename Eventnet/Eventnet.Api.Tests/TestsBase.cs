using System;
using System.Net.Http;
using Eventnet.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Eventnet.Api.Tests;

public class TestsBase
{
    protected HttpClient HttpClient => factory.CreateClient();
    private readonly WebApplicationFactory<Program> factory = new TestWebApplicationFactory<Program>();

    protected void ApplyToDb(Action<ApplicationDbContext> action)
    {
        var scopeFactory = factory.Server.Host.Services.GetService<IServiceScopeFactory>();
        using var scope = scopeFactory!.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        action(context!);
    }

    protected T ApplyToDb<T>(Func<ApplicationDbContext, T> action)
    {
        var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
        using var scope = scopeFactory!.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        return action(context!);
    }
}