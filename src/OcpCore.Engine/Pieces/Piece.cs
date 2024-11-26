using OcpCore.Engine.General;

namespace OcpCore.Engine.Pieces;

public abstract class Piece
{
    public abstract int Value { get; }
    
    public abstract void GetMoves(Board board, int position, Colour colour, List<int> moveList);

    protected static void GetDirectionalMoves(Board board, int position, Colour colour, List<int> moveList, params (int RankDelta, int FileDelta)[] directions)
    {
        var rank = Cell.GetRank(position);

        var file = Cell.GetFile(position);
        
        for (var i = 0; i < directions.Length; i++)
        {
            var direction = directions[i];
            
            for (var distance = 1; distance <= Constants.MaxMoveDistance; distance++)
            {
                var newRank = rank + distance * direction.RankDelta;

                var newFile = file + distance * direction.FileDelta;

                var cell = Cell.GetCell(newRank, newFile);

                if (cell < 0)
                {
                    break;
                }

                var content = board[cell];

                if (content == 0)
                {
                    moveList.Add(cell);
                    
                    continue;
                }

                if ((content & (byte) colour) > 0)
                {
                    break;
                }

                moveList.Add(cell);
                
                break;    
            }
        }
    }
}