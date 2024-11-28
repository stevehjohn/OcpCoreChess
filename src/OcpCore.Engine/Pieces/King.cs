using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class King : Piece
{
    public override Kind Kind => Kind.King;
    
    public override int Value => 0;
        
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        CheckForCastlingOpportunities(board, position, colour, moveList);
        
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
                moveList.Add(new Move(position, cell, MoveOutcome.Move));

                continue;
            }

            if (Cell.Colour(content) != colour)
            {
                moveList.Add(new Move(position, cell, MoveOutcome.Capture));
            }
        }
    }

    private static void CheckForCastlingOpportunities(Board board, int position, Colour colour, List<Move> moveList)
    {
        if (board.IsKingInCheck(colour))
        {
            return;
        }

        var castleSide = colour == Colour.White ? Castle.White : Castle.Black;
        
        if ((board.State.CastleStatus & castleSide) == 0)
        {
            return;
        }
        
        var rankStart = colour == Colour.White ? 0 : Constants.BlackRankCellStart;
        
        if (board[rankStart + Files.LeftKnight] == 0 && board[rankStart + Files.LeftBishop] == 0 && board[rankStart + Files.Queen] == 0)
        {
            if ((board.State.CastleStatus & (colour == Colour.White ? Castle.WhiteQueenSide : Castle.BlackQueenSide)) > 0)
            {
                if (! board.IsKingInCheck(colour, position - 1))
                {
                    moveList.Add(new Move(position, rankStart + Files.LeftBishop, MoveOutcome.Move));
                }
            }
        }
        
        if (board[rankStart + Files.RightBishop] == 0 && board[rankStart + Files.RightKnight] == 0)
        {
            if ((board.State.CastleStatus & (colour == Colour.White ? Castle.WhiteKingSide : Castle.BlackKingSide)) > 0)
            {
                if (! board.IsKingInCheck(colour, position + 1))
                {
                    moveList.Add(new Move(position, rankStart + Files.RightKnight, MoveOutcome.Move));
                }
            }
        }
    }
}