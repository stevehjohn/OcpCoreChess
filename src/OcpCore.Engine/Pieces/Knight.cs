using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public class Knight : Piece
{
    public override Kind Kind => Kind.Knight;
    
    public override int Value => 30;
    
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        var rank = Cell.GetRank(position);

        var file = Cell.GetFile(position);
        
        for (var i = 0; i < Constants.KnightMoves.Length; i++)
        {
            var direction = Constants.KnightMoves[i];

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