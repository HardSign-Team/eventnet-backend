using Eventnet.Domain.Events;

namespace Eventnet.Infrastructure.Validators;

public interface IEventValidator
{
    EventCreationValidationResult Validate(Event eventForValidation);
}