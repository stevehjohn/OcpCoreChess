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
            if (board[Files.LeftKnight] == 0 && board[Files.LeftBishop] == 0 && board[Files.Queen] == 0)
            {
                if ((board.State.CastleStatus & Castle.WhiteQueenSide) > 0)
                {
                    moveList.Add(new Move(Files.LeftBishop, false));
                }
            }

            if (board[Files.RightBishop] == 0 && board[Files.RightKnight] == 0)
            {
                if ((board.State.CastleStatus & Castle.WhiteKingSide) > 0)
                {
                    moveList.Add(new Move(Files.RightKnight, false));
                }
            }
        }

        if (colour == Colour.Black)
        {
            if ((board.State.CastleStatus & Castle.BlackQueenSide) > 0)
            {
                if (board[Constants.BlackRankCellStart + Files.LeftKnight] == 0 && board[Constants.BlackRankCellStart + Files.LeftBishop] == 0 && board[Constants.BlackRankCellStart + Files.Queen] == 0)
                {
                    moveList.Add(new Move(Constants.BlackRankCellStart + Files.LeftBishop, false));
                }
            }

            if ((board.State.CastleStatus & Castle.BlackKingSide) > 0)
            {
                if (board[Constants.BlackRankCellStart + Files.RightBishop] == 0 && board[Constants.BlackRankCellStart + Files.RightKnight] == 0)
                {
                    if ((board.State.CastleStatus & Castle.BlackKingSide) > 0)
                    {
                        moveList.Add(new Move(Constants.BlackRankCellStart + Files.RightKnight, false));
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