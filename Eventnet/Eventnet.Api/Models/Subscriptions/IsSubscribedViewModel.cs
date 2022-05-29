namespace Eventnet.Api.Models.Subscriptions;

public record IsSubscribedViewModel(Guid EventId, bool IsSubscribed);