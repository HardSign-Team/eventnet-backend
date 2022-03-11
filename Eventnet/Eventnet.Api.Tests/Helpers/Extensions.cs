using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Eventnet.Api.Tests.Helpers;

public static class Extensions
{
    public static string[] GetRequiredHeader(this HttpResponseMessage response, string headerName)
    {
        var hasResponseHeader = response.Headers.TryGetValues(headerName, out var responseHeaderValues);
        var hasContentHeader = response.Content.Headers.TryGetValues(headerName, out var contentHeaderValues);

        if (hasResponseHeader && hasContentHeader)
            Assert.Fail($"Should have only one '{headerName}' header");

        if (hasResponseHeader)
        {
            return responseHeaderValues!.ToArray();
        }

        if (hasContentHeader)
        {
            return contentHeaderValues!.ToArray();
        }

        Assert.Fail($"Should have '{headerName}' header");
        throw new InvalidOperationException();
    }

    public static void ShouldHaveHeader(this HttpResponseMessage response, string headerName, string headerValue)
    {
        var actualHeaderValue = GetRequiredHeader(response, headerName);
        actualHeaderValue.Should().BeEquivalentTo(headerValue);
    }

    public static void ShouldNotHaveHeader(this HttpResponseMessage response, string headerName)
    {
        var hasResponseHeader = response.Headers.TryGetValues(headerName, out var _);
        var hasContentHeader = response.Content.Headers.TryGetValues(headerName, out var _);
        var hasHeader = hasResponseHeader || hasContentHeader;

        hasHeader.Should().BeFalse();
    }

    public static JToken ReadContentAsJson(this HttpResponseMessage response)
    {
        var content = response.Content.ReadAsStringAsync().Result;
        return JToken.Parse(content);
    }
    
    public static ByteArrayContent SerializeToJsonContent(this object obj,
        string contentType = "application/json")
    {
        var json = JsonConvert.SerializeObject(obj);
        var bytes = Encoding.UTF8.GetBytes(json);
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return content;
    }

    public static void ShouldHaveJsonContentEquivalentTo(this HttpResponseMessage response, object expected)
    {
        var content = response.ReadContentAsJson();
        content.Should().BeEquivalentTo(JToken.FromObject(expected));
    }
}