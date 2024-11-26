using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public class Knight : Piece
{
    public override int Value => 30;
    
    public override void GetMoves(Board board, int position, Colour colour, List<int> moveList)
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
                moveList.Add(cell);
                    
                continue;
            }

            if ((Colour) (content & (byte) colour) != colour)
            {
                moveList.Add(cell);
            }
        }
    }
}