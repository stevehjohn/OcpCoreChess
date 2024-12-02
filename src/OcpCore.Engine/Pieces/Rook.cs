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

        var positionBit = 1ul << position;
        
        var mask = Moves[Kind.Rook][MoveSet.Horizontal][position] & ~positionBit;
        
        // Remove blockers, account for opponents

        moves |= mask;

        mask = Moves[Kind.Rook][MoveSet.Vertical][position] & ~positionBit;
        
        // Remove blockers, account for opponents

        moves |= mask;
        
        return moves;
    }
}