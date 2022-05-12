using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.UnitTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.PhotoControllerTests;

[TestFixture]
public class GetPhotosShould : PhotoApiTestsBase
{
    [TestCase("")]
    [TestCase("trash")]
    public async Task Response404_WhenNotFound(string eventId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, BuildGetPhotosUri(eventId));

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestCase("00000000-0000-0000-0000-000000000000")]
    public async Task Response400_WhenNotGuid(string eventId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, BuildGetPhotosUri(eventId));

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Response200_WhenEventExists()
    {
        FillDatabase();
        var entity = ApplyToDb(context => context.Events.First());
        var request = new HttpRequestMessage(HttpMethod.Get, BuildGetPhotosUri(entity.Id.ToString()));

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var photos = response.ReadContentAs<string[]>();
        photos.Should().NotBeEmpty();
        photos.Should().NotContain(x => string.IsNullOrEmpty(x));
    }

    private void FillDatabase()
    {
        ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users.ToList());
            context.AddPhotos(context.Events.ToList());
            for (var i = 0; i < 3; i++)
                context.AddPhotos(context.Events.ToList());
        });
    }
}