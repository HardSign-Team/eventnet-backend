using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models;

public record RequestEventsModel([Required] Location Location);