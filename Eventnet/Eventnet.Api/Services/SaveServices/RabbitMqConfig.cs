namespace Eventnet.Services.SaveServices;

public class RabbitMqConfig
{
    public string Queue { get; set; }
    public string HostName { get; set; }
    public int RecommendedMessageSize { get; set; }
}