using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public abstract class Piece
{
    public abstract Kind Kind { get; }
    
    public abstract int Value { get; }

    public abstract void GetMoves(Board board, int position, Colour colour, List<Move> moveList);

    protected void GetDirectionalMoves(Board board, int position, Colour colour, List<Move> moveList, params (int RankDelta, int FileDelta)[] directions)
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

                if (! board.IsOccupied(cell))
                {
                    moveList.Add(new Move(position, cell, MoveOutcome.Move, 0));
                    
                    continue;
                }

                if (board.IsColour(cell, colour))
                {
                    break;
                }

                moveList.Add(new Move(position, cell, MoveOutcome.Capture, PieceCache.Get(content).Value * 10 + Value));
                
                break;    
            }
        }
    }
}