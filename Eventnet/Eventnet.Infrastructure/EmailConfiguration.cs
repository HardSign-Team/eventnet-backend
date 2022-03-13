namespace Eventnet.Infrastructure;

public record EmailConfiguration
{
    public string Host { get; init; }
    public int Port { get; init; }
    public string Login { get; init; }
    public string Password { get; init; }
}