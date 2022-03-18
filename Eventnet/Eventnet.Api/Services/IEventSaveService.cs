using Eventnet.Models;

namespace Eventnet.Services;

public interface IEventSaveService
{
    void Save(Event savedEvent, IFormFile[] photos);
    bool IsEventSaved(Guid id, out string exceptionValue);
}