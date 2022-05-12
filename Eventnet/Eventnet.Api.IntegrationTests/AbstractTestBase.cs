using System;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Authentication;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Eventnet.Api.IntegrationTests;

public abstract class AbstractTestBase
{
    protected HttpClient HttpClient => Factory.CreateClient();
    protected abstract WebApplicationFactory<Program> Factory { get; }
    protected IServiceScopeFactory GetScopeFactory() => Factory.Services.GetService<IServiceScopeFactory>()!;

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

    protected async Task ApplyToDbAsync(Func<ApplicationDbContext, Task> action)
    {
        var scopeFactory = Factory.Services.GetService<IServiceScopeFactory>();
        using var scope = scopeFactory!.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        await action(context!);
    }

    protected async Task<T> ApplyToDbAsync<T>(Func<ApplicationDbContext, Task<T>> action)
    {
        var scopeFactory = Factory.Services.GetService<IServiceScopeFactory>();
        using var scope = scopeFactory!.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        return await action(context!);
    }

    protected async Task<(UserEntity, HttpClient)> CreateAuthorizedClient(
        string username = "TestUser", string password = "TestPassword")
    {
        var factory = GetScopeFactory();
        using var scope = factory.CreateScope();
        var userManager = scope.ServiceProvider.GetService<UserManager<UserEntity>>()!;
        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            var registerModel = new RegisterModel
            {
                UserName = username,
                Email = $"{username}@test.com",
                Password = password,
                ConfirmPassword = password,
                Gender = Gender.Male,
                PhoneNumber = null
            };

            user = await AuthorizationHelper.RegisterUserAsync(userManager, registerModel);
        }

        var client = await AuthorizationHelper.AuthorizeClient(HttpClient, username, password);
        return (user, client);
    }
}