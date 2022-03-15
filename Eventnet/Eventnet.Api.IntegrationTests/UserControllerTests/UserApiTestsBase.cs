using System;
using System.Web;
using Eventnet.Api.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http.Extensions;

namespace Eventnet.Api.IntegrationTests.UserControllerTests;

public class UserApiTestsBase : TestsBase
{
    private const string BaseRoute = "/api/users";

    protected Uri BuildSearchEventsUri(string? prefix, int maxUsers = 100)
    {
        var qb = new QueryBuilder { { "maxUsers", maxUsers.ToString() } };
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/searchByPrefix/{HttpUtility.UrlEncode(prefix)}",
            Query = qb.ToString(),
        };
        return uriBuilder.Uri;
    }
}