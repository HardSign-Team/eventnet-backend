using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Events;
using Eventnet.Domain.Events;
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
        var (eventInfo, formFiles) = model;
        var multiContent = new MultipartFormDataContent();

        foreach (var (content, name) in GetInfoContent(eventInfo))
        {
            multiContent.Add(content, $"{nameof(CreateEventModel.Info)}.{name}");
        }

        foreach (var photo in formFiles)
        {
            var fs = photo.OpenReadStream();
            var fileStreamContent = GetFileStreamContent(fs, photo.Headers.ContentType[0] ?? throw new Exception());
            multiContent.Add(fileStreamContent, "Photos", photo.FileName);
        }

        return multiContent;
    }

    private static IEnumerable<(HttpContent, string)> GetInfoContent(EventInfoModel eventInfo)
    {
        yield return (new StringContent(eventInfo.EventId.ToString()), nameof(EventInfoModel.EventId));

        yield return (new StringContent(eventInfo.StartDate.ToString(CultureInfo.CurrentCulture)),
            nameof(EventInfoModel.StartDate));

        if (eventInfo.EndDate is { } endDate)
            yield return (new StringContent(endDate.ToString(CultureInfo.CurrentCulture)),
                nameof(EventInfoModel.EndDate));

        yield return (new StringContent(eventInfo.Name), nameof(EventInfoModel.Name));

        if (eventInfo.Description is { } description)
            yield return (new StringContent(description), nameof(EventInfoModel.Description));

        foreach (var tag in eventInfo.Tags)
            yield return (new StringContent(tag), nameof(EventInfoModel.Tags));

        const string locationName = nameof(EventInfoModel.Location);
        yield return (new StringContent(eventInfo.Location.Latitude.ToString(CultureInfo.InvariantCulture)),
            $"{locationName}.{nameof(Location.Latitude)}");
        yield return (new StringContent(eventInfo.Location.Longitude.ToString(CultureInfo.InvariantCulture)),
            $"{locationName}.{nameof(Location.Longitude)}");
    }

    private static StreamContent GetFileStreamContent(Stream fileStream, string mediaType)
    {
        var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        return content;
    }
}