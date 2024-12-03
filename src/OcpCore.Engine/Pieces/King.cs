using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.Pieces;

public class King : Piece
{
    public override Kind Kind => Kind.King;
    
    public override int Value => 0;

    public King(Moves moves) : base(moves)
    {
    }

    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moves = Moves[Kind.King][MoveSet.Specific][position];

        moves &= ~game[colour];
        
        return moves;
    }
}