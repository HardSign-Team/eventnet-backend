using Eventnet.Infrastructure;
using Eventnet.Models;

namespace Eventnet.Services.SaveServices;

public interface IEventSaveService
{
    Task RequestSave(Event eventForSave, IFormFile[] photos);
    SaveEventResult GetSaveEventResult(Guid id);
    bool IsHandling(Guid id);
}