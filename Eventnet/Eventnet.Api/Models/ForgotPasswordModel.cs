using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models;

public record ForgotPasswordModel([EmailAddress] string Email);