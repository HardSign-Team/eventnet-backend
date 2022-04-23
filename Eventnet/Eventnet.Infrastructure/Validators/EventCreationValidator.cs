using Eventnet.Domain;
using Eventnet.Domain.Events;

namespace Eventnet.Infrastructure.Validators;

public class EventCreationValidator : IEventCreationValidator
{
    private readonly IPhotoValidator photoValidator;
    private readonly IEventValidator eventValidator;

    public EventCreationValidator(IPhotoValidator photoValidator, IEventValidator eventValidator)
    {
        this.photoValidator = photoValidator;
        this.eventValidator = eventValidator;
    }

    public EventCreationValidationResult Validate(List<Photo> photos, Event eventForValidation)
    {
        var (photoValidationResult, photoValidationErrorMessage) = photoValidator.Validate(photos);
        var (eventValidationResult, eventValidationErrorMessage) = eventValidator.Validate(eventForValidation);
        return new EventCreationValidationResult(photoValidationResult && eventValidationResult,
            FormatException(photoValidationErrorMessage, eventValidationErrorMessage));
    }

    private static string FormatException(string photoErrorMessage, string eventErrorMessage) =>
        (photoErrorMessage + " " + eventErrorMessage).Trim();
}