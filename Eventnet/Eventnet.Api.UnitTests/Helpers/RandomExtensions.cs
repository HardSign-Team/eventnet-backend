using System;
using System.Collections.Generic;

namespace Eventnet.Api.UnitTests.Helpers;

public static class RandomExtensions
{
    public static T Choice<T>(this Random random, IList<T> collection)
    {
        return collection[random.Next(collection.Count)];
    }
}