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
                moveList.Add(new Move(position, cell, false));

                continue;
            }

            if (Cell.Colour(content) != colour)
            {
                moveList.Add(new Move(position, cell, true));
            }
        }
    }

    private static void CheckForCastlingOpportunities(Board board, int position, Colour colour, List<Move> moveList)
    {
        var rank = colour == Colour.White ? Ranks.WhiteHomeRank : Ranks.BlackHomeRank;

        var offset = rank * Constants.Files;
        
        if (board[offset + Files.LeftKnight] == 0 && board[offset + Files.LeftBishop] == 0 && board[offset + Files.Queen] == 0)
        {
            if ((board.State.CastleStatus & (colour == Colour.White ? Castle.WhiteQueenSide : Castle.BlackQueenSide)) > 0)
            {
                if (! board.IsKingInCheck(colour, position - 1))
                {
                    moveList.Add(new Move(position, Files.LeftBishop, false));
                }
            }
        }

        if (board[offset + Files.RightBishop] == 0 && board[offset + Files.RightKnight] == 0)
        {
            if ((board.State.CastleStatus & (colour == Colour.White ? Castle.WhiteKingSide : Castle.BlackKingSide)) > 0)
            {
                if (! board.IsKingInCheck(colour, position + 1))
                {
                    moveList.Add(new Move(position, Files.RightKnight, false));
                }
            }
        }
    }
}