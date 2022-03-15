using Eventnet.Domain.Events.Filters.Data;

namespace Eventnet.Models;

public record DateFilterModel(DateTime Border, DateEquality DateEquality);