using Eventnet.Helpers.EventFilters;

namespace Eventnet.Models;

public record DateFilterModel(DateTime Border, DateEquality DateEquality);