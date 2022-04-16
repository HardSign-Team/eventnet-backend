using System.Text.Json.Serialization;

namespace Eventnet.Api.Models.Subscriptions;

public record SubscriptionsCountViewModel(
    [property: JsonPropertyName("eventId")] Guid EventId,
    [property: JsonPropertyName("count")] int Count);