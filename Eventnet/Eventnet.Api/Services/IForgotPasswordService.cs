namespace Eventnet.Services;

public interface IForgotPasswordService
{
    Task SendCodeAsync(string email);
    bool VerifyCode(string email, string code);
}