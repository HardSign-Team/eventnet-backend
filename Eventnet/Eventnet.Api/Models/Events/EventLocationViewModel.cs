using Eventnet.Domain.Events;

namespace Eventnet.Api.Models.Events;

public record EventLocationViewModel(Guid Id, Location Location, string Name);