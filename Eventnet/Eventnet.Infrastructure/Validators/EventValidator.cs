using Eventnet.Domain.Events;

namespace Eventnet.Infrastructure.Validators;

public class EventValidator : IEventValidator
{
    public EventCreationValidationResult Validate(Event eventForValidation) =>
        // надо придумать что проверять на бэкенде.
        new (true, string.Empty);
}