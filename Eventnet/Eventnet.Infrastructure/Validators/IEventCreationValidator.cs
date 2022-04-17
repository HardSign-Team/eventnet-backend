using Eventnet.Domain;
using Eventnet.Domain.Events;

namespace Eventnet.Infrastructure.Validators;

public interface IEventCreationValidator
{
    EventCreationValidationResult Validate(List<Photo> photos, Event eventForValidation);
}