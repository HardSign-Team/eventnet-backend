using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Authentication;
using Eventnet.DataAccess.Entities;
using Eventnet.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.SubscriptionsControllerTests;

public class SubscriptionsApiTestsBase : TestsBase
{
    private const string BaseRoute = "/api/subscriptions";

    [TearDown]
    public void TearDown()
    {
        ApplyToDb(context => context.Clear());
    }

    protected Uri BuildCountOnEventUri(string? eventId)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/count/{HttpUtility.UrlEncode(eventId)}"
        };
        return uriBuilder.Uri;
    }

    protected Uri BuildSubscribeQuery(Guid eventId)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/subscribe/{HttpUtility.UrlEncode(eventId.ToString())}"
        };
        return uriBuilder.Uri;
    }

    protected Uri BuildUnsubscribeQuery(Guid eventId)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/unsubscribe/{HttpUtility.UrlEncode(eventId.ToString())}"
        };
        return uriBuilder.Uri;
    }

    protected async Task<(UserEntity, HttpClient)> CreateAuthorizedClient(string username, string password)
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