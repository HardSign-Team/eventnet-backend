using System;
using System.Collections.Generic;

namespace Eventnet.Api.IntegrationTests.Helpers;

public static class RandomExtensions
{
    public static T Choice<T>(this Random random, IList<T> collection)
    {
        return collection[random.Next(collection.Count)];
    }
}