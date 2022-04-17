using Eventnet.Domain;

namespace Eventnet.Infrastructure.Validators;

public interface IPhotoValidator
{
    EventCreationValidationResult Validate(List<Photo> photos);
}