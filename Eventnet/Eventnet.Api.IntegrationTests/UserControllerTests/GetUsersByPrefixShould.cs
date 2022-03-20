using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.DataAccess.Entities;
using Eventnet.Models;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.UserControllerTests;

public class GetUsersByPrefixShould : UserApiTestsBase
{
    [Test]
    public async Task ResponseCode200_WhenEmptyList()
    {
        var request = new HttpRequestMessage();
        request.Headers.Add("Accept", "application/json");
        request.RequestUri = BuildSearchEventsUri("AAA");

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShouldHaveJsonContentEquivalentTo(new UserNameListModel(0, Array.Empty<UserNameModel>()));
    }

    [Test]
    public async Task ResponseCode400_WhenIncorrectPrefix()
    {
        var request = new HttpRequestMessage();
        request.Headers.Add("Accept", "application/json");
        request.RequestUri = BuildSearchEventsUri(null);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ResponseCode200_WhenFoundNames()
    {
        var names = new[]
        {
            "Volodimir Zelensky",
            "Vladimir Putin",
            "Vladimir Molodec",
            "Slavsya Vladimir",
            "Vasya Vot-Suka"
        };
        CreateUsers(names);
        var request = new HttpRequestMessage();
        request.Headers.Add("Accept", "application/json");
        request.RequestUri = BuildSearchEventsUri("Vl");

        var response = await HttpClient.SendAsync(request);

        var result = JsonConvert.DeserializeObject<UserNameListModel>(response.Content.ReadAsStringAsync().Result);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Count.Should().Be(2);
        result.Models.Select(x => x.UserName).Should().Contain(names[1..3]);
    }

    private void CreateUsers(string[] names)
    {
        var fixture = new Fixture();
        var users = names.Select(name => CreateUserWithName(fixture, name));
        ApplyToDb(context =>
        {
            context.Users.AddRange(users);
            context.SaveChanges();
        });
    }

    private UserEntity CreateUserWithName(Fixture fixture, string name)
    {
        return fixture.Build<UserEntity>().With(x => x.UserName, name).Create();
    }
}