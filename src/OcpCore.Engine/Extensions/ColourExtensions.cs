using System.Runtime.CompilerServices;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.Extensions;

public static class ColourExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Colour Invert(this Colour colour)
    {
        return colour == Colour.Black ? Colour.White : Colour.Black;
    }
}