﻿using System.ComponentModel.DataAnnotations;
using Eventnet.Domain.Events;

namespace Eventnet.Api.Models.Filtering;

public record LocationFilterModel(
    Location Location,
    [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Radius should be positive.")]
    double Radius);