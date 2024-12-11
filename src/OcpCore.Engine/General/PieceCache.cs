using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public class PieceCache
{
    private static readonly Lazy<PieceCache> Instantiator = new(Instantiate);
    
    public static PieceCache Instance => Instantiator.Value;

    private readonly Pieces _pieces;

    public Piece this[Kind kind] => _pieces[(int) kind];
    
    private PieceCache()
    {
        _pieces[(int) Kind.Pawn] = new Pawn();
        _pieces[(int) Kind.Rook] = new Rook();
        _pieces[(int) Kind.Knight] = new Knight();
        _pieces[(int) Kind.Bishop] = new Bishop();
        _pieces[(int) Kind.Queen] = new Queen();
        _pieces[(int) Kind.King] = new King();
    }

    private static PieceCache Instantiate()
    {
        return new PieceCache();
    }
}