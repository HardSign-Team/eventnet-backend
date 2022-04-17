using Eventnet.Domain.Selectors;

namespace Eventnet.Api.Models.Events;

public record EventNameListModel(int TotalCount, EventName[] Models);