using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Events;
using Microsoft.AspNetCore.Http.Extensions;

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
        var multiContent = new MultipartFormDataContent();

        foreach (var (content, name) in GetInfoContent(model))
        {
            multiContent.Add(content, $"{name}");
        }

        var formFiles = model.Photos;
        if (formFiles is not null)
        {
            foreach (var photo in formFiles)
            {
                var fs = photo.OpenReadStream();
                var fileStreamContent = GetFileStreamContent(fs, photo.Headers.ContentType[0] ?? throw new Exception());
                multiContent.Add(fileStreamContent, nameof(CreateEventModel.Photos), photo.FileName);
            }
        }

        return multiContent;
    }

    private static IEnumerable<(HttpContent, string)> GetInfoContent(CreateEventModel eventInfo)
    {
        yield return (new StringContent(eventInfo.EventId.ToString()), nameof(CreateEventModel.EventId));
        yield return (new StringContent(eventInfo.StartDate.ToString(CultureInfo.CurrentCulture)),
            nameof(CreateEventModel.StartDate));
        if (eventInfo.EndDate is { } endDate)
            yield return (new StringContent(endDate.ToString(CultureInfo.CurrentCulture)),
                nameof(CreateEventModel.EndDate));
        yield return (new StringContent(eventInfo.Name), nameof(CreateEventModel.Name));
        yield return (new StringContent(eventInfo.Description), nameof(CreateEventModel.Description));
        foreach (var tag in eventInfo.Tags ?? Array.Empty<string>())
            yield return (new StringContent(tag), nameof(CreateEventModel.Tags));
        yield return (new StringContent(eventInfo.Latitude.ToString(CultureInfo.InvariantCulture)),
            nameof(CreateEventModel.Latitude));
        yield return (new StringContent(eventInfo.Longitude.ToString(CultureInfo.InvariantCulture)),
            nameof(CreateEventModel.Longitude));
    }

    private static StreamContent GetFileStreamContent(Stream fileStream, string mediaType)
    {
        var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        return content;
    }
}