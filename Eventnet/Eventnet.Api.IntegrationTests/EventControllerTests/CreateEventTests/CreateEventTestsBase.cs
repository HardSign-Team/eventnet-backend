using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.DataAccess;
using Eventnet.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.EventControllerTests.CreateEventTests;

public class CreateEventTestsBase : TestWithRabbitMqBase
{
    protected const string BaseRoute = "/api/events";
    private string token;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        token = await GetAccessToken();
    }

    protected HttpClient GetAuthorizedClient()
    {
        var client = HttpClient;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

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

    protected async Task<Guid> GetEventGuid()
    {
        var client = GetAuthorizedClient();
        var request = CreateDefaultRequestToId();
        var response = await client.SendAsync(request);
        var guid = await response.Content.ReadAsStringAsync();
        return Guid.Parse(guid.Replace("\"", ""));
    }

    protected static MultipartFormDataContent GetEventCreationRequestMessage(Guid eventId, Guid ownerId, string filename, string mediaType)
    {
        var multiContent = new MultipartFormDataContent()
        {
            { new StringContent(eventId.ToString()), "Id" },
            { new StringContent(ownerId.ToString()), "OwnerId" },
            { new StringContent(DateTime.Now.ToString(CultureInfo.InvariantCulture)), "StartDate" },
            { new StringContent(DateTime.Now.ToString(CultureInfo.InvariantCulture)), "EndDate" },
            { new StringContent("TestEvent"), "Name" },
            { new StringContent("TestDescription"), "Description" },
            { new StringContent(JsonConvert.SerializeObject(new Location(0, 0))), "Location" }
        };
        var fileStreamContent = GetFileStreamContent(filename, mediaType);
        multiContent.Add(fileStreamContent, "Photos", filename);
        
        return multiContent;
    }

    private static StreamContent GetFileStreamContent(string filename, string mediaType)
    {
        var content = new StreamContent(File.OpenRead(filename));
        content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        return content;
    }

    private async Task<string> GetAccessToken()
    {
        const string username = "TestUser";
        const string password = "12345678";
        
        CreateTestUser(username);
        var request = GetLoginRequest(username, password);
        var response = await HttpClient.SendAsync(request);
        var loginModel = JsonConvert.DeserializeObject<LoginResponseModel>(response.Content.ReadAsStringAsync().Result);
        return loginModel.AccessToken;
    }
    
    private void CreateTestUser(string username)
    {
        ApplyToDb(context =>
        {
            context.Users.Add(new UserEntity()
            {
                UserName = username, 
                Email = "TestUser@example.com", 
                Id = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(), 
                EmailConfirmed = true, 
                PasswordHash = "AQAAAAEAACcQAAAAEG61NA2ZZp6xzxqrMvfOgbKU1P+pJEuu2Rt6IiHgJuXQcWQukjPJ9cbR22n5h7T/RA==", // 12345678
                NormalizedUserName = username.ToUpper()
            });
            context.SaveChanges();
        });
    }

    private static HttpRequestMessage GetLoginRequest(string username, string password)
    {
        var request = new HttpRequestMessage();
        var login = new LoginModel(username, password);
        request.Method = HttpMethod.Post;
        request.Content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
        request.RequestUri = new UriBuilder(Configuration.BaseUrl)
        {
            Path = "/api/auth/login"
        }.Uri;
        return request;
    }

    protected static HttpRequestMessage CreateDefaultRequestToId()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/guid"
        }.Uri;
        return request;
    }
}