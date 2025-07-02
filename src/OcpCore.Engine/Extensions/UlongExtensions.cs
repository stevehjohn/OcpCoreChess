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

    public static int PopRandomBit(this ref ulong value)
    {
        if (value == 0)
            return -1;

        var setCount = BitOperations.PopCount(value);

        var target = Random.Shared.Next(setCount);

        var mask = value;

        while (target-- > 0)
        {
            mask &= mask - 1;
        }

        var selected = mask & ~(mask - 1);

        var index = BitOperations.TrailingZeroCount(selected);

        value ^= selected;

        return index;
    }
}