using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Rook : Piece
{
    public override Kind Kind => Kind.Rook;
    
    public override int Value => 5;
    
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        GetDirectionalMoves(board, position, colour, moveList, Constants.OrthogonalMoves);
    }
}