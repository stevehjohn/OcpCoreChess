using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public static class PieceCache
{
    private static readonly Piece[] Pieces;
    
    static PieceCache()
    {
        Pieces =
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

    public static Piece Get(Kind kind) => Pieces[(int) kind];
}