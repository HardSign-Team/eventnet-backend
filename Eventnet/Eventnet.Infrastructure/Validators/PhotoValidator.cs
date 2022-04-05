using System.Drawing;

namespace Eventnet.Infrastructure.Validators;

public class PhotoValidator : IPhotoValidator
{
    public EventCreationValidationResult Validate(List<Image> photos) =>
        // надо придумать что проверять на бэкенде.
        new ("", true);
}