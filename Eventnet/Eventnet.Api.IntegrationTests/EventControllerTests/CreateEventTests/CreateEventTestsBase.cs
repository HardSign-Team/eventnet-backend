﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Authentication;
using Eventnet.DataAccess.Entities;
using Eventnet.DataAccess.Models;
using Eventnet.Domain.Events;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

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

    protected async Task<Guid> GetEventGuid()
    {
        var (_, client) = await CreateAuthorizedClient("TestUser", "123456");
        var request = CreateDefaultRequestToId();
        var response = await client.SendAsync(request);
        var guid = await response.Content.ReadAsStringAsync();
        return Guid.Parse(guid.Replace("\"", ""));
    }
    
    protected async Task<(UserEntity, HttpClient)> CreateAuthorizedClient(string username, string password)
    {
        var factory = GetScopeFactory();
        using var scope = factory.CreateScope();
        var userManager = scope.ServiceProvider.GetService<UserManager<UserEntity>>()!;
        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            var registerModel = new RegisterModel
            {
                UserName = username,
                Email = $"{username}@test.com",
                Password = password,
                ConfirmPassword = password,
                Gender = Gender.Male,
                PhoneNumber = null
            };

            user = await AuthorizationHelper.RegisterUserAsync(userManager, registerModel);
        }

        var client = await AuthorizationHelper.AuthorizeClient(HttpClient, username, password);
        return (user, client);
    }

    protected static FileStream GetFileStream(string path) => File.OpenRead(path);

    protected static MultipartFormDataContent GetEventCreationRequestMessage(Guid eventId, Guid ownerId, FileStream fileStream, string mediaType)
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
        var fileStreamContent = GetFileStreamContent(fileStream, mediaType);
        var filename = fileStream.Name.Split(Path.PathSeparator).Last();
        multiContent.Add(fileStreamContent, "Photos", filename);
        
        return multiContent;
    }

    private static StreamContent GetFileStreamContent(FileStream fileStream, string mediaType)
    {
        var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        return content;
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
}