using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Knight : Piece
{
    public override int Value => Scores.Knight;
    
    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moves = Moves[MoveSet.Knight][position];

        moves &= ~game[colour];
        
        return moves;
    }
}