using Eventnet.Models;

namespace Eventnet.Infrastructure.Validators;

public interface IEventValidator
{
    EventCreationValidationResult Validate(Event eventForValidation);
}