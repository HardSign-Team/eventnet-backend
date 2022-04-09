using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.EventControllerTests.CreateEventTests;

public class CreateEventTestsShould : CreateEventTestsBase
{
    private const string PathToPhoto = "test.png";
    private const string PathToText = "notImage.txt";
    private const string ImageMediaTypePng = "image/png";
    private const string TextMediaType = "text/plain";

    [Test]
    public async Task TestGetId()
    {
        var request = CreateDefaultRequestToId();
        var client = GetAuthorizedClient();
        
        var response = await client.SendAsync(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Test]
    public async Task ResponseCode202_WhenEventIsCorrect()
    {
        var response = await PostAsync(Guid.NewGuid(), Guid.NewGuid(), PathToPhoto, ImageMediaTypePng);
        
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Test]
    public async Task ResponseCode400_WhenFileTypeIsNotSupported()
    {
        var response = await PostAsync(Guid.NewGuid(), Guid.NewGuid(), PathToText, TextMediaType);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<HttpResponseMessage> PostAsync(Guid eventId, Guid ownerId, string filename, string mediaType)
    {
        var client = GetAuthorizedClient();
        var multipart = GetEventCreationRequestMessage(eventId, ownerId, filename, mediaType);
        var uri = new UriBuilder(Configuration.BaseUrl) { Path = BaseRoute }.Uri;
        return await client.PostAsync(uri, multipart);
    }

    [Test]
    public async Task SaveEvent_WhenAllCorrect()
    {
        var client = GetAuthorizedClient();
        
        var eventId = await GetEventGuid();
        
        await PostAsync(eventId, Guid.NewGuid(), PathToPhoto, ImageMediaTypePng);
        var saveToDbTime = Task.Delay(1000);
        await saveToDbTime;
        
        var request = GetIsCreatedRequest(eventId);
        
        var response = await client.SendAsync(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var eventEntity = ApplyToDb(context => context.Events.Find(eventId));
        eventEntity.Should().NotBeNull();
    }

    [Test]
    public async Task IsCreatedResponseCode202_WhenNotSaveYet()
    {
        var client = GetAuthorizedClient();
        var eventId = Guid.NewGuid();
        await PostAsync(eventId, Guid.NewGuid(), PathToPhoto, ImageMediaTypePng);
        var request = GetIsCreatedRequest(eventId);
        
        var response = await client.SendAsync(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
    
    
    [Test]
    public async Task IsCreatedResponseCode400_WhenUnknownGuid()
    {
        var client = GetAuthorizedClient();
        var eventId = Guid.NewGuid();
        var request = GetIsCreatedRequest(eventId);
        
        var response = await client.SendAsync(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}