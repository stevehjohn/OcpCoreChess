using System.Numerics;

namespace OcpCore.Engine.Extensions;

public static class UlongExtensions
{
    public static int PopBit(this ref ulong value)
    {
        var zeros = BitOperations.TrailingZeroCount(value);

        if (zeros == 64)
        {
            return -1;
        }

        value ^= 1ul << zeros;

        return zeros;
    }
}