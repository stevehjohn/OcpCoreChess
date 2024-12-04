using System.Runtime.CompilerServices;
using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.Extensions;

public static class ColourExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Colour Invert(this Colour colour)
    {
        return 1 - colour;
    }
}