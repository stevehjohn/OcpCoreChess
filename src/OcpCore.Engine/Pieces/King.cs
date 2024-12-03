using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class King : Piece
{
    public override Kind Kind => Kind.King;
    
    public override int Value => Scores.King;

    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moves = Moves[MoveSet.King][position];

        moves &= ~game[colour];
        
        return moves;
    }
}