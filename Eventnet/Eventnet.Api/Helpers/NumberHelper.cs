namespace Eventnet.Helpers;

public static class NumberHelper
{
    public static int Normalize(int x, int min, int max = int.MaxValue) => Math.Max(min, Math.Min(x, max));
}