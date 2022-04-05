using Eventnet.Models;

namespace Eventnet.Infrastructure.Validators;

public class EventValidator : IEventValidator
{
    public EventCreationValidationResult Validate(Event eventForValidation) =>
        // надо придумать что проверять на бэкенде.
        new ("", true);
}