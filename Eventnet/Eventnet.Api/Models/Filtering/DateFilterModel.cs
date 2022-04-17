using Eventnet.Domain.Events.Filters.Data;

namespace Eventnet.Api.Models.Filtering;

public record DateFilterModel(DateTime Border, DateEquality DateEquality);