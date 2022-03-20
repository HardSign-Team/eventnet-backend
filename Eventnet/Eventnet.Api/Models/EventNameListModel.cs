using Eventnet.Domain.Selectors;

namespace Eventnet.Models;

public record EventNameListModel(int TotalCount, EventName[] Models);