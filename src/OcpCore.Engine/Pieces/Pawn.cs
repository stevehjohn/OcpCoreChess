using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public class Pawn : Piece
{
    public override int Value => 10;
    
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        var rank = Cell.GetRank(position);

        var file = Cell.GetFile(position);

        var direction = colour == Colour.Black ? - 1 : 1;
    }
}