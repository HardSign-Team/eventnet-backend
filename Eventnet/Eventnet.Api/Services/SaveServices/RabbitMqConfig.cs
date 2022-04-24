#pragma warning disable CS8618
// Used for Configuration

namespace Eventnet.Api.Services.SaveServices;

public class RabbitMqConfig
{
    public string Queue { get; set; }
    public string HostName { get; set; }
    public int RecommendedMessageSizeInBytes { get; set; }
}