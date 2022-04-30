using Eventnet.Domain.Selectors;

namespace Eventnet.Api.Models.Events;

public record EventNameListViewModel(int TotalCount, EventName[] Models);