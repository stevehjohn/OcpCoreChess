using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public class King : Piece
{
    public override Kind Kind => Kind.King;
    
    public override int Value => 0;
        
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        if (colour == Colour.White)
        {
            // TODO: Replace magic numbers with constants
            if (board[2] == 0 && board[3] == 0 && board[4] == 0)
            {
                if ((board.State.CastleStatus & Castle.WhiteQueenSide) > 0)
                {
                    moveList.Add(new Move(2, false));
                }
            }

            if (board[5] == 0 && board[6] == 0)
            {
                if ((board.State.CastleStatus & Castle.WhiteKingSide) > 0)
                {
                    moveList.Add(new Move(6, false));
                }
            }
        }

        if (colour == Colour.Black)
        {
            if ((board.State.CastleStatus & Castle.BlackQueenSide) > 0)
            {
                if (board[57] == 0 && board[58] == 0 && board[59] == 0)
                {
                    moveList.Add(new Move(58, false));
                }
            }

            if ((board.State.CastleStatus & Castle.BlackKingSide) > 0)
            {
                if (board[61] == 0 && board[62] == 0)
                {
                    if ((board.State.CastleStatus & Castle.BlackKingSide) > 0)
                    {
                        moveList.Add(new Move(62, false));
                    }
                }
            }
        }
        
        var rank = Cell.GetRank(position);

        var file = Cell.GetFile(position);
        
        for (var i = 0; i < Constants.DirectionalMoves.Length; i++)
        {
            var direction = Constants.DirectionalMoves[i];

            var newRank = rank + direction.RankDelta;

            var newFile = file + direction.FileDelta;

            var cell = Cell.GetCell(newRank, newFile);

            if (cell < 0)
            {
                continue;
            }

            var content = board[cell];

            if (content == 0)
            {
                moveList.Add(new Move(cell, false));

                continue;
            }

            if ((Colour) (content & Constants.ColourMask) != colour)
            {
                moveList.Add(new Move(cell, true));
            }
        }
    }
}