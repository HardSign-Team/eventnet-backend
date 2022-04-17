namespace Eventnet.Infrastructure;

public enum EventSaveStatus
{
    Saved,
    NotSavedDueToServerError,
    NotSavedDueToUserError,
    InProgress
}