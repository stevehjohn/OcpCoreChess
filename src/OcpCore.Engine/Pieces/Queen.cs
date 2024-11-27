using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Queen : Piece
{
    public override Kind Kind => Kind.Queen;
    
    public override int Value => 90;
    
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        GetDirectionalMoves(board, position, colour, moveList, Constants.DirectionalMoves);
    }
}