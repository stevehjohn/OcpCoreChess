using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public class Bishop : Piece
{
    public override int Value => 30;
    
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        GetDirectionalMoves(board, position, colour, moveList, Constants.DiagonalMoves);
    }
}