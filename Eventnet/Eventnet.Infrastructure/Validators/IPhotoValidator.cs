using System.Drawing;

namespace Eventnet.Infrastructure.Validators;

public interface IPhotoValidator
{
    EventCreationValidationResult Validate(List<Image> photos);
}