using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models;

public record LocationFilterModel(
    Location Location, 
    [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Radius should be positive.")] double Radius);