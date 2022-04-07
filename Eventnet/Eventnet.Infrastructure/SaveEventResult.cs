namespace Eventnet.Infrastructure;

public record SaveEventResult(EventSaveStatus Status, string ExceptionInformation);