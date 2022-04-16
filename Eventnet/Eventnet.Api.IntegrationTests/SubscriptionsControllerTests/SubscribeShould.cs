using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Authentication;
using Eventnet.DataAccess.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.SubscriptionsControllerTests;

public class SubscribeShould : SubscriptionsApiTestsBase
{
    
    
    [Test]
    public async Task Response401_WhenUserNotAuthorized()
    {
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return context.Events.First();
        });
        var request = BuildSubscribeRequest(entity.Id);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Test]
    public async Task Response404_WhenEventNotFound()
    {
        var client = await CreateAuthorizedClient("TestUser", "TestPassword");
        var request = BuildSubscribeRequest(Guid.Empty);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(10)]
    public async Task Response200_WhenSubscribeNTimes(int n)
    {
        var client = await CreateAuthorizedClient("TestUser", "TestPassword");
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return context.Events.First();
        });
        var request = BuildSubscribeRequest(entity.Id);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShouldHaveJsonContentEquivalentTo(new
        {
            eventId = entity.Id,
            count = 1
        });
    }

    private async Task<HttpClient> CreateAuthorizedClient(string username, string password)
    {
        var factory = GetScopeFactory();
        using var scope = factory.CreateScope();
        var userManager = scope.ServiceProvider.GetService<UserManager<UserEntity>>()!;
        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            var registerModel = new RegisterModel(username, $"{username}@test.com", password, null);
            await AuthorizationHelper.RegisterUserAsync(userManager, registerModel);
        }
        return await AuthorizationHelper.AuthorizeClient(HttpClient, username, password);
    }
    
    private HttpRequestMessage BuildSubscribeRequest(Guid entityId) => new(HttpMethod.Post, BuildSubscribeQuery(entityId));
}