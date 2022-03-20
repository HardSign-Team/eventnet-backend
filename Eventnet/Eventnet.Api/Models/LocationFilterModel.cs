using System.ComponentModel.DataAnnotations;
using Eventnet.Domain.Events;

namespace Eventnet.Models;

public record LocationFilterModel(
    Location Location, 
    [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Radius should be positive.")] double Radius);