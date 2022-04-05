using System.Drawing;
using Eventnet.Models;

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

    public EventCreationValidationResult Validate(List<Image> photos, Event eventForValidation)
    {
        var (photoValidationException, photoValidationResult) = photoValidator.Validate(photos);
        var (eventValidationException, eventValidationResult) = eventValidator.Validate(eventForValidation);
        return new EventCreationValidationResult(
            FormatException(photoValidationException, eventValidationException),
            photoValidationResult && eventValidationResult);
    }

    private string FormatException(string photoException, string eventException) => (photoException + " " + eventException).Trim();
}