using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Eventnet.Api.Models.Authentication;
using Eventnet.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Eventnet.Api.IntegrationTests.Helpers;

public class AuthorizationHelper
{
    public static async Task<UserEntity> RegisterUserAsync(
        UserManager<UserEntity> userManager,
        RegisterModel registerModel)
    {
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = registerModel.UserName,
            NormalizedUserName = registerModel.UserName,
            Email = registerModel.Email,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(user, registerModel.Password);
        if (!result.Succeeded)
            throw new Exception();
        return user;
    }

    public static async Task<HttpClient> AuthorizeClient(HttpClient httpClient, string username, string password)
    {
        var request = BuildRequestLogin(username, password);
        var response = await httpClient.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        var loginResult = JsonConvert.DeserializeObject<LoginResult>(result);
        var token = loginResult.Tokens.AccessToken;
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return httpClient;
    }

    private static HttpRequestMessage BuildRequestLogin(string username, string password)
    {
        var request = new HttpRequestMessage();
        var login = new LoginModel(username, password);
        request.Method = HttpMethod.Post;
        request.Content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = "/api/auth/login"
        };
        request.RequestUri = uriBuilder.Uri;
        return request;
    }
}