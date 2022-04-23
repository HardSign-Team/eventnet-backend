using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.EventControllerTests.CreateEventTests;

[Explicit]
public class CreateEventShould : CreateEventTestsBase
{
    private const string PathToPhoto = "test.png";
    private const string PathToText = "notImage.txt";
    private const string ImageMediaTypePng = "image/png";
    private const string TextMediaType = "text/plain";

    [Test]
    public async Task TestGetId()
    {
        var request = CreateDefaultRequestToId();
        var (_, client) = await CreateAuthorizedClient("TestUser", "123456");

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ResponseCode202_WhenEventIsCorrect()
    {
        var photo = GetFileStream(PathToPhoto);

        var response = await PostAsync(Guid.NewGuid(), Guid.NewGuid(), photo, ImageMediaTypePng);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Test]
    public async Task ResponseCode400_WhenFileTypeIsNotSupported()
    {
        var text = GetFileStream(PathToText);

        var response = await PostAsync(Guid.NewGuid(), Guid.NewGuid(), text, TextMediaType);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ResponseCode400_WhenSameIdProvidedTwoTimes()
    {
        var photo = GetFileStream(PathToPhoto);

        var id = Guid.NewGuid();
        await PostAsync(id, Guid.NewGuid(), photo, ImageMediaTypePng);
        var response2 = await PostAsync(id, Guid.NewGuid(), photo, ImageMediaTypePng);

        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task SaveEvent_WhenAllCorrect()
    {
        var (_, client) = await CreateAuthorizedClient("TestUser", "123456");
        var photo = GetFileStream(PathToPhoto);

        var eventId = await GetEventGuid();

        await PostAsync(eventId, Guid.NewGuid(), photo, ImageMediaTypePng);
        var saveToDbTime = Task.Delay(1000);
        await saveToDbTime;

        var request = GetIsCreatedRequest(eventId);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var eventEntity = ApplyToDb(context => context.Events.Find(eventId));
        eventEntity.Should().NotBeNull();
        eventEntity!.Id.Should().Be(eventId);

        var photoEntities = ApplyToDb(context => context.Photos.ToList()).Where(x => x.EventId == eventId);
        photoEntities.Should().NotBeNull();
    }

    [Test]
    public async Task IsCreatedResponseCode202_WhenNotSaveYet()
    {
        var (_, client) = await CreateAuthorizedClient("TestUser", "123456");
        var eventId = Guid.NewGuid();
        var photo = GetFileStream(PathToPhoto);

        await PostAsync(eventId, Guid.NewGuid(), photo, ImageMediaTypePng);
        var request = GetIsCreatedRequest(eventId);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Test]
    public async Task IsCreatedResponseCode400_WhenUnknownGuid()
    {
        var (_, client) = await CreateAuthorizedClient("TestUser", "123456");
        var eventId = Guid.NewGuid();
        var request = GetIsCreatedRequest(eventId);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<HttpResponseMessage> PostAsync(
        Guid eventId,
        Guid ownerId,
        FileStream fileStream,
        string mediaType)
    {
        var (_, client) = await CreateAuthorizedClient("TestUser", "123456");
        var multipart = GetEventCreationRequestMessage(eventId, ownerId, fileStream, mediaType);
        var uri = new UriBuilder(Configuration.BaseUrl) { Path = BaseRoute }.Uri;
        return await client.PostAsync(uri, multipart);
    }
}