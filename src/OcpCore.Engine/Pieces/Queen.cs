using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public class Queen : Piece
{
    public override int Value => 90;
    
    public override void GetMoves(Board board, int position, byte colour, List<int> moveList)
    {
        GetDirectionalMoves(board, position, colour, moveList, Constants.DirectionalMoves);
    }
}