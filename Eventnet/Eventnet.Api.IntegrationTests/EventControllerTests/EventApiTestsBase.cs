using System;
using System.Web;
using Eventnet.Api.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http.Extensions;

namespace Eventnet.Api.IntegrationTests.EventControllerTests;

public abstract class EventApiTestsBase : TestsBase
{
    private const string BaseRoute = "/api/events";

    protected Uri BuildEventsByIdUri(Guid guid) => BuildEventsByIdUri(guid.ToString());

    protected Uri BuildEventsByIdUri(string guid)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/{HttpUtility.UrlEncode(guid)}"
        };
        return uriBuilder.Uri;
    }

    protected Uri BuildEventsPageUri(int? pageNumber, int? pageSize)
    {
        var query = new QueryBuilder();
        if (pageNumber.HasValue)
            query.Add("pageNumber", pageNumber.Value.ToString());
        if (pageSize.HasValue)
            query.Add("pageSize", pageSize.Value.ToString());
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = BaseRoute,
            Query = query.ToString()
        };
        return uriBuilder.Uri;
    }
}