using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public class Pawn : Piece
{
    public override Kind Kind => Kind.Pawn;
    
    public override int Value => 10;
    
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        var rank = Cell.GetRank(position);

        var file = Cell.GetFile(position);

        var direction = colour == Colour.Black ? - 1 : 1;

        var cell = Cell.GetCell(rank + direction * 2, file);
        
        if ((rank == Constants.BlackPawnRank && colour == Colour.Black) || (rank == Constants.WhitePawnRank && colour == Colour.White))
        {
            if (board[cell] == 0 && board[cell - direction * 8] == 0)
            {
                moveList.Add(new Move(cell, false));
            }
        }

        cell = Cell.GetCell(rank + direction, file);
        
        if (board[cell] == 0)
        {
            moveList.Add(new Move(cell, false));
        }

        cell = Cell.GetCell(rank + direction, file - 1);

        if (cell >= 0)
        {
            var content = board[cell];

            if (cell == board.State.EnPassantTarget || (content > 0 && (Colour) (content & Masks.ColourMask) != colour))
            {
                moveList.Add(new Move(cell, true));
            }
        }

        cell = Cell.GetCell(rank + direction, file + 1);

        if (cell >= 0)
        {
            var content = board[cell];

            if (cell == board.State.EnPassantTarget || (content > 0 && (Colour) (content & Masks.ColourMask) != colour))
            {
                moveList.Add(new Move(cell, true));
            }
        }
    }
}