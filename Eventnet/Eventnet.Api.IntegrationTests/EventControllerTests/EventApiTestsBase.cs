using System;
using System.Buffers.Text;
using System.Text;
using System.Web;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace Eventnet.Api.IntegrationTests.EventControllerTests;

public abstract class EventApiTestsBase : TestsBase
{
    private const string BaseRoute = "/api/events";

    protected Uri BuildEventsByIdUri(Guid guid) => BuildEventsByIdUri(guid.ToString());

    protected Uri BuildEventsByIdUri(string? guid)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/{HttpUtility.UrlEncode(guid)}"
        };
        return uriBuilder.Uri;
    }

    protected Uri BuildDeleteEventUri(string? guid)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/{HttpUtility.UrlEncode(guid)}"
        };
        return uriBuilder.Uri;
    }

    protected Uri BuildEventsPageUri(EventsFilterModel filterModel, int? pageNumber, int? pageSize)
    {
        var query = new QueryBuilder();
        var filterBytes = Encoding.Default.GetBytes(JsonConvert.SerializeObject(filterModel));
        query.Add("f", Convert.ToBase64String(filterBytes));
        if (pageNumber.HasValue)
            query.Add("p", pageNumber.Value.ToString());
        if (pageSize.HasValue)
            query.Add("ps", pageSize.Value.ToString());
        
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = BaseRoute,
            Query = query.ToString()
        };
        return uriBuilder.Uri;
    }
}