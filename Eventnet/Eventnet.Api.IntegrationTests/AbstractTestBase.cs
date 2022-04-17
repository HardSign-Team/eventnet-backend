﻿using System;
using System.Net.Http;
using Eventnet.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Eventnet.Api.IntegrationTests;

public abstract class AbstractTestBase 
{
    protected HttpClient HttpClient => Factory.CreateClient();
    protected abstract WebApplicationFactory<Program> Factory { get; }
    
    protected void ApplyToDb(Action<ApplicationDbContext> action)
    {
        var scopeFactory = Factory.Services.GetService<IServiceScopeFactory>();
        using var scope = scopeFactory!.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        action(context!);
    }

    protected T ApplyToDb<T>(Func<ApplicationDbContext, T> action)
    {
        var scopeFactory = Factory.Services.GetService<IServiceScopeFactory>();
        using var scope = scopeFactory!.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        return action(context!);
    }
}