using Eventnet.Domain;

namespace Eventnet.Infrastructure.Validators;

public class PhotoValidator : IPhotoValidator
{
    public EventCreationValidationResult Validate(List<Photo> photos) =>
        // надо придумать что проверять на бэкенде.
        new(true, string.Empty);
}