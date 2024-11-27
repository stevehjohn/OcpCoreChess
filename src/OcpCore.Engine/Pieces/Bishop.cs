using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Bishop : Piece
{
    public override Kind Kind => Kind.Bishop;
    
    public override int Value => 30;
    
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        GetDirectionalMoves(board, position, colour, moveList, Constants.DiagonalMoves);
    }
}