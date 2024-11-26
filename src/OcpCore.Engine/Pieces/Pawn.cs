using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public class Pawn : Piece
{
    public override int Value => 10;
    
    public override void GetMoves(Board board, int position, Colour colour, List<int> moveList)
    {
        throw new NotImplementedException();
    }
}