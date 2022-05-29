using Eventnet.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace Eventnet.Api.Services;

public class ForgotPasswordService : IForgotPasswordService
{
    private readonly IEmailService emailService;
    private readonly IMemoryCache cache;

    public ForgotPasswordService(IEmailService emailService, IMemoryCache cache)
    {
        this.emailService = emailService;
        this.cache = cache;
    }

    public Task SendCodeAsync(string email)
    {
        var code = GenerateCode();

        cache.Set(email, code, TimeSpan.FromMinutes(2));

        return emailService.SendEmailAsync(email,
            "Restore password",
            $"Ваш код: {code}");
    }

    public bool VerifyCode(string email, string code)
        => cache.TryGetValue(email, out string cachedCode) && cachedCode == code;

    private static string GenerateCode()
    {
        var rnd = new Random();
        return rnd.Next(0, 1000000).ToString("D6");
    }
}