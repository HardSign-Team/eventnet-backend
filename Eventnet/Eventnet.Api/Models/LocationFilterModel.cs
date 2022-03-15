using Eventnet.Domain.Events.Filters.Data;

namespace Eventnet.Models;

public record LocationFilterModel(Location Location, double Radius);