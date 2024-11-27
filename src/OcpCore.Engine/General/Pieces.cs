using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public static class Pieces
{
    private static readonly Piece[] PieceCache;
    
    static Pieces()
    {
        PieceCache =
        [
            null,
            new Pawn(),
            new Rook(),
            new Knight(),
            new Bishop(),
            new Queen(),
            new King()
        ];
    }

    public static Piece Get(byte kind) => PieceCache[kind & Masks.Kind];
}