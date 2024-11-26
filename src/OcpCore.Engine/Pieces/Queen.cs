using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public class Queen : Piece
{
    public override Kind Kind => Kind.Knight;
    
    public override int Value => 90;
    
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        GetDirectionalMoves(board, position, colour, moveList, Constants.DirectionalMoves);
    }
}