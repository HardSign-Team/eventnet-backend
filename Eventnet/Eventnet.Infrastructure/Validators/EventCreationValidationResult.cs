namespace Eventnet.Infrastructure.Validators;

public record EventCreationValidationResult(bool IsOk, string ErrorMessage);