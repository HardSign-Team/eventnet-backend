using System.ComponentModel.DataAnnotations;

namespace Eventnet.Api.Models.Authentication;

public record ForgotPasswordModel([EmailAddress] string Email);