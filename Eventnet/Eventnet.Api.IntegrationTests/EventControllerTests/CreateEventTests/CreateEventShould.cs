using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Events;
using Eventnet.DataAccess.Entities;
using Eventnet.DataAccess.Extensions;
using Eventnet.Domain.Events;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        var (_, client) = await CreateAuthorizedClient();

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ResponseCode202_WhenEventIsCorrect()
    {
        var (_, client) = await CreateAuthorizedClient();
        var photo = GetFileStream(PathToPhoto);

        var response = await PostAsync(client, Guid.NewGuid(), Guid.NewGuid(), photo, ImageMediaTypePng);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Test]
    public async Task ResponseCode400_WhenFileTypeIsNotSupported()
    {
        var (_, client) = await CreateAuthorizedClient();
        var text = GetFileStream(PathToText);

        var response = await PostAsync(client, Guid.NewGuid(), Guid.NewGuid(), text, TextMediaType);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ResponseCode400_WhenSameIdProvidedTwoTimes()
    {
        var (_, client) = await CreateAuthorizedClient();
        var photo = GetFileStream(PathToPhoto);

        var id = Guid.NewGuid();
        await PostAsync(client, id, Guid.NewGuid(), photo, ImageMediaTypePng);
        var response2 = await PostAsync(client, id, Guid.NewGuid(), photo, ImageMediaTypePng);

        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task SaveEvent_WhenAllCorrect()
    {
        var (user, client) = await CreateAuthorizedClient();
        await using var photo = GetFileStream(PathToPhoto);

        var eventId = await GetEventGuid(client);

        var model = CreateDefaultCreateEventModel() with
        {
            Id = eventId,
            OwnerId = Guid.NewGuid().ToString(),
            Photos = new[] { CreateFormFile(photo, ImageMediaTypePng) }
        };
        await PostAsync(client, model);
        await Task.Delay(1000);

        var request = GetIsCreatedRequest(eventId);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var eventEntity = ApplyToDb(context => context.Events.Include(x => x.Tags)
            .FirstOrDefault(x => x.Id == eventId));
        AssertSave(eventEntity ?? throw new Exception(), user, model);
        var photoEntities = ApplyToDb(context => context.Photos.ForEvent(eventId).ToList());
        photoEntities.Should().NotBeEmpty();
    }

    [Test]
    public async Task IsCreatedResponseCode202_WhenNotSaveYet()
    {
        var (_, client) = await CreateAuthorizedClient();
        var eventId = Guid.NewGuid();
        await using var photo = GetFileStream(PathToPhoto);

        await PostAsync(client, eventId, Guid.NewGuid(), photo, ImageMediaTypePng);
        var request = GetIsCreatedRequest(eventId);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Test]
    public async Task IsCreatedResponseCode400_WhenUnknownGuid()
    {
        var (_, client) = await CreateAuthorizedClient();
        var eventId = Guid.NewGuid();
        var request = GetIsCreatedRequest(eventId);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<HttpResponseMessage> PostAsync(
        HttpClient client,
        Guid eventId,
        Guid ownerId,
        FileStream fileStream,
        string mediaType)
    {
        var model = CreateDefaultCreateEventModel() with
        {
            Id = eventId,
            OwnerId = ownerId.ToString(),
            Photos = new[] { CreateFormFile(fileStream, mediaType) }
        };
        return await PostAsync(client, model);
    }

    private static IFormFile CreateFormFile(FileStream stream, string mediaType)
    {
        IFormFile formFile = new FormFile(stream,
            0,
            stream.Length,
            nameof(CreateEventModel.Photos),
            stream.Name.Split(Path.PathSeparator).Last())
        {
            Headers = new HeaderDictionary(),
            ContentType = mediaType
        };
        return formFile;
    }

    private static CreateEventModel CreateDefaultCreateEventModel()
    {
        return new CreateEventModel(Guid.NewGuid(),
            Guid.NewGuid().ToString(),
            DateTime.Now,
            DateTime.Now,
            "Test",
            "Description",
            new Location(0, 0),
            new[] { "Tag1", "Tag2", "Tag3" },
            Array.Empty<IFormFile>());
    }
    
    private async Task<HttpResponseMessage> PostAsync(HttpClient client, CreateEventModel model)
    {
        var multipart = GetEventCreationRequestMessage(model);
        var uri = new UriBuilder(Configuration.BaseUrl) { Path = BaseRoute }.Uri;
        return await client.PostAsync(uri, multipart);
    }
    
    
    private static void AssertSave(EventEntity eventEntity, UserEntity owner, CreateEventModel model)
    {
        eventEntity.Id.Should().Be(model.Id);
        eventEntity.Name.Should().Be(model.Name);
        eventEntity.Description.Should().Be(model.Description);
        eventEntity.Tags.Select(x => x.Name).OrderBy(x => x).Should().Equal(model.Tags.OrderBy(x => x));
        eventEntity.StartDate.Should().Be(model.StartDate);
        eventEntity.EndDate.Should().Be(model.EndDate);
        eventEntity.OwnerId.Should().Be(owner.Id);
    }
}