using Eventnet.Domain.Selectors;

namespace Eventnet.Api.Models;

public record EventNameListModel(int TotalCount, EventName[] Models);