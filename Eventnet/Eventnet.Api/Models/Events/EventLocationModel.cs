using Eventnet.Domain.Events;

namespace Eventnet.Api.Models.Events;

public record EventLocationModel(Guid Id, Location Location, string Name);