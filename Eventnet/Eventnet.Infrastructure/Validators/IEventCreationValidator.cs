using System.Drawing;
using Eventnet.Models;

namespace Eventnet.Infrastructure.Validators;

public interface IEventCreationValidator
{
    EventCreationValidationResult Validate(List<Image> photos, Event eventForValidation);
}