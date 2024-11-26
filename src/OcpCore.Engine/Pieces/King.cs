using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public class King : Piece
{
    public override Kind Kind => Kind.King;
    
    public override int Value => 0;
        
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        var rank = Cell.GetRank(position);

        var file = Cell.GetFile(position);

        if (rank == Constants.WhiteHomeRank)
        {
            if ((board.State.CastleStatus & Castle.WhiteQueenSide) > 0)
            {
                moveList.Add(new Move(Cell.GetCell(rank, file - 2), false));
            }

            if ((board.State.CastleStatus & Castle.WhiteQueenSide) > 0)
            {
                moveList.Add(new Move(Cell.GetCell(rank, file + 2), false));
            }
        }

        if (rank == Constants.BlackHomeRank)
        {
            if ((board.State.CastleStatus & Castle.BlackQueenSide) > 0)
            {
                moveList.Add(new Move(Cell.GetCell(rank, file - 2), false));
            }

            if ((board.State.CastleStatus & Castle.BlackQueenSide) > 0)
            {
                moveList.Add(new Move(Cell.GetCell(rank, file + 2), false));
            }
        }
        
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