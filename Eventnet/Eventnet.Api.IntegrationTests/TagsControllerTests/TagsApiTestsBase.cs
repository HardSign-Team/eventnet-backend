using System;
using Microsoft.AspNetCore.Http.Extensions;

namespace Eventnet.Api.IntegrationTests.TagsControllerTests;

public class TagsApiTestsBase : TestsBase
{
    private const string BaseUrl = "api/tags";
    
    public Uri BuildSearchByName(object name, int maxCount)
    {
        var qb = new QueryBuilder { { "mc", maxCount.ToString() } };
        var builder = new UriBuilder()
        {
            Path = $"{BaseUrl}/search-by-name/{name}",
            Query = qb.ToString()
        };
        return builder.Uri;
    }
}