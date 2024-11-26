using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public class Rook : Piece
{
    public override Kind Kind => Kind.Rook;
    
    public override int Value => 50;
    
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        GetDirectionalMoves(board, position, colour, moveList, Constants.OrthogonalMoves);
    }
}