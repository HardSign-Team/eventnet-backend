using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models.Authentication;

public record ForgotPasswordModel([EmailAddress] string Email);