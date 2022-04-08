﻿namespace Eventnet.Domain.Events;

public record Event(
    Guid Id,
    string OwnerId,
    DateTime StartDate,
    DateTime? EndDate,
    string Name,
    string Description,
    Location Location);