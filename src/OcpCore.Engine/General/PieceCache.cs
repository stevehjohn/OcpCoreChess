using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public static class PieceCache
{
    private static readonly Piece[] Pieces;

    private static readonly Moves Moves = new();
    
    static PieceCache()
    {
        Pieces =
        [
            null,
            new Pawn(Moves),
            new Rook(Moves),
            new Knight(Moves),
            new Bishop(Moves),
            new Queen(Moves),
            new King(Moves)
        ];
    }

    public static Piece Get(Kind kind) => Pieces[(int) kind];
}