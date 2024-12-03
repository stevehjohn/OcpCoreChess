using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.Pieces;

public class Knight : Piece
{
    public override Kind Kind => Kind.Knight;
    
    public override int Value => 3;
    
    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moves = Moves[Kind.Knight][MoveSet.Specific][position];

        moves &= ~game[colour];
        
        return moves;
    }
}