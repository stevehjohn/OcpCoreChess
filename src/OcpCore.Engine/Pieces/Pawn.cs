using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Pawn : Piece
{
    public override Kind Kind => Kind.Pawn;
    
    public override int Value => 1;
    
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        var rank = Cell.GetRank(position);

        var file = Cell.GetFile(position);

        var direction = colour == Colour.Black ? ColourDirections.Black : ColourDirections.White;

        var cell = Cell.GetCell(rank + direction * 2, file);
        
        if ((rank == Ranks.BlackPawnRank && colour == Colour.Black) || (rank == Ranks.WhitePawnRank && colour == Colour.White))
        {
            if (board[cell] == 0 && board[cell - direction * 8] == 0)
            {
                moveList.Add(new Move(position, cell, MoveOutcome.Move, 0));
            }
        }

        cell = Cell.GetCell(rank + direction, file);
        
        if (board[cell] == 0)
        {
            moveList.Add(new Move(position, cell, MoveOutcome.Move, 0));
        }

        cell = Cell.GetCell(rank + direction, file - 1);

        if (cell >= 0)
        {
            var content = board[cell];

            if (cell == board.State.EnPassantTarget || (content > 0 && Cell.Colour(content) != colour))
            {
                moveList.Add(new Move(position, cell, MoveOutcome.Capture, Value * 10 + Value));
            }
        }

        cell = Cell.GetCell(rank + direction, file + 1);

        if (cell >= 0)
        {
            var content = board[cell];

            if (cell == board.State.EnPassantTarget || (content > 0 && Cell.Colour(content) != colour))
            {
                moveList.Add(new Move(position, cell, MoveOutcome.Capture, Value * 10 + Value));
            }
        }
    }
}