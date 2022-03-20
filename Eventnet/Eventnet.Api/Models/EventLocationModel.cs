using Eventnet.Domain.Events;

namespace Eventnet.Api.Models;

public record EventLocationModel(Guid Id, Location Location, string Name);