using System.Runtime.CompilerServices;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Bitboards;

[InlineArray(Constants.Planes)]
public struct Planes
{
    private ulong _plane;

    public const int White = (int) Colour.White;

    public const int Black = (int) Colour.Black;

    public const int Pawn = (int) Kind.Pawn;

    public const int Rook = (int) Kind.Rook;

    public const int Knight = (int) Kind.Knight;

    public const int Bishop = (int) Kind.Bishop;

    public const int Queen = (int) Kind.Queen;

    public const int King = (int) Kind.King;
}