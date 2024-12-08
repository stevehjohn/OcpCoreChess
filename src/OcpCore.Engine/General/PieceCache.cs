using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public class PieceCache
{
    private static readonly Lazy<PieceCache> Instantiator = new(Instantiate);
    
    public static PieceCache Instance => Instantiator.Value;

    private readonly Piece[] _pieces;

    public Piece this[Kind kind] => _pieces[(int) kind];
    
    private PieceCache()
    {
        _pieces =
        [
            null,
            null,
            new Pawn(),
            new Rook(),
            new Knight(),
            new Bishop(),
            new Queen(),
            new King()
        ];
    }

    private static PieceCache Instantiate()
    {
        return new PieceCache();
    }
}