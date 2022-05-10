using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Events;
using Eventnet.Domain.Events;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace Eventnet.Api.IntegrationTests.EventControllerTests.CreateEventTests;

public class CreateEventTestsBase : TestWithRabbitMqBase
{
    protected const string BaseRoute = "/api/events";

    protected static HttpRequestMessage GetIsCreatedRequest(Guid eventId)
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        var query = new QueryBuilder { { "id", eventId.ToString() } };
        request.RequestUri = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/is-created",
            Query = query.ToString()
        }.Uri;
        return request;
    }
    
    protected static HttpRequestMessage CreateDefaultRequestToId()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/request-event-creation"
        }.Uri;
        return request;
    }

    protected async Task<Guid> GetEventGuid(HttpClient client)
    {
        var request = CreateDefaultRequestToId();
        var response = await client.SendAsync(request);
        return response.ReadContentAs<Guid>();
    }

    protected static FileStream GetFileStream(string path) => File.OpenRead(path);

    protected static MultipartFormDataContent GetEventCreationRequestMessage(CreateEventModel model)
    {
        var multiContent = new MultipartFormDataContent
        {
            { new StringContent(model.Id.ToString()), nameof(CreateEventModel.Id) },
            { new StringContent(model.OwnerId), nameof(CreateEventModel.OwnerId) },
            {
                new StringContent(model.StartDate.ToString(CultureInfo.InvariantCulture)),
                nameof(CreateEventModel.StartDate)
            },
            { new StringContent(model.Name), nameof(CreateEventModel.Name) },
            { new StringContent(JsonConvert.SerializeObject(new Location(0, 0))), nameof(CreateEventModel.Location) }
        };
        if (model.EndDate is { } endDate)
        {
            multiContent.Add(new StringContent(endDate.ToString(CultureInfo.InvariantCulture)),
                nameof(CreateEventModel.EndDate));
        }

        if (model.Description is { } description)
        {
            multiContent.Add(new StringContent(description), nameof(CreateEventModel.Description));
        }

        foreach (var photo in model.Photos)
        {
            var fs = photo.OpenReadStream();
            var fileStreamContent = GetFileStreamContent(fs, photo.Headers.ContentType[0] ?? throw new Exception());
            multiContent.Add(fileStreamContent, photo.Name, photo.FileName);
        }


        return multiContent;
    }

    private static StreamContent GetFileStreamContent(Stream fileStream, string mediaType)
    {
        var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        return content;
    }
}