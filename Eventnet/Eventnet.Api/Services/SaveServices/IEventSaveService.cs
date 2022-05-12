using Eventnet.Domain.Events;
using Eventnet.Infrastructure;

namespace Eventnet.Api.Services.SaveServices;

public interface IEventSaveService
{
    Task RequestSave(EventInfo eventForSave, IFormFile[] photos);
    SaveEventResult GetSaveEventResult(Guid id);
    bool IsHandling(Guid id);
}