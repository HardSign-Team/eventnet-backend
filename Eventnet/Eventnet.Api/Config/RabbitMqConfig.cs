#pragma warning disable CS8618
// Used for Configuration

namespace Eventnet.Api.Config;

public class RabbitMqConfig
{
    public string QueueEventSave { get; set; }
    public string QueueEventUpdate { get; set; }
    public string HostName { get; set; }
    public int RecommendedMessageSizeInBytes { get; set; }
    public int Port { get; set; }
}