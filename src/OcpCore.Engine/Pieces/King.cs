using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.Pieces;

public class King : Piece
{
    public override Kind Kind => Kind.King;
    
    public override int Value => 0;

    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moves = Moves[MoveSet.King][position];

        moves &= ~game[colour];
        
        return moves;
    }
}