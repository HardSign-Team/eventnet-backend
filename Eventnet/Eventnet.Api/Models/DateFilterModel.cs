using Eventnet.Domain.Events.Filters.Data;

namespace Eventnet.Api.Models;

public record DateFilterModel(DateTime Border, DateEquality DateEquality);