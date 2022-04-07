using Eventnet.Infrastructure;
using Eventnet.Models;

namespace Eventnet.Services.SaveServices;

public interface IEventSaveService
{
    Task SaveAsync(Event eventForSave, IFormFile[] photos);
    SaveEventResult GetSaveEventResult(Guid id);
}