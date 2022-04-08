using Eventnet.Domain;
using Eventnet.Models;

namespace Eventnet.Infrastructure.Validators;

public interface IEventCreationValidator
{
    EventCreationValidationResult Validate(List<Photo> photos, Event eventForValidation);
}