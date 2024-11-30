using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Knight : Piece
{
    public override Kind Kind => Kind.Knight;
    
    public override int Value => 3;
    
    public override void GetMoves(Board board, int position, Colour colour, List<Move> moveList)
    {
        var rank = Cell.GetRank(position);

        var file = Cell.GetFile(position);
        
        for (var i = 0; i < Constants.KnightMoves.Length; i++)
        {
            var direction = Constants.KnightMoves[i];

            var newRank = rank + direction.RankDelta;

            var newFile = file + direction.FileDelta;

            var target = Cell.GetCell(newRank, newFile);

            if (target < 0)
            {
                continue;
            }

            if (! board.IsOccupied(target))
            {
                moveList.Add(new Move(position, target, MoveOutcome.Move, 0));
                    
                continue;
            }

            if (! board.IsColour(target, colour))
            {
                moveList.Add(new Move(position, target, MoveOutcome.Capture, PieceCache.Get(board[target]).Value * 10 + Value));
            }
        }
    }
}