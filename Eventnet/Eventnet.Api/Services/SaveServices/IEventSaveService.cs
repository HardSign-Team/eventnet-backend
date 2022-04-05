using Eventnet.Models;

namespace Eventnet.Services.SaveServices;

public interface IEventSaveService
{
    Task SaveAsync(Event savedEvent, IFormFile[] photos);
    bool IsEventSaved(Guid id, out string exceptionValue);
}