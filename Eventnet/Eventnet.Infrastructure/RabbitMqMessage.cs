﻿using Eventnet.Models;

namespace Eventnet.Infrastructure;

public record RabbitMqMessage(Event Event, List<byte[]> Photos);