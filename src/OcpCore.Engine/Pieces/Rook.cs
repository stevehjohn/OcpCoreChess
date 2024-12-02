using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.Pieces;

public class Rook : Piece
{
    public override Kind Kind => Kind.Rook;
    
    public override int Value => 5;

    public Rook(Moves moves) : base(moves)
    {
    }

    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moves = 0ul;

        var mask = Moves[Kind.Rook][MoveSet.Horizontal][position];
        
        var path = (long) mask & (long) game[colour] & -(long) game[colour];

        moves |= (ulong) path;
        
        return moves;
    }
}