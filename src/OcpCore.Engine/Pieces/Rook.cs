using System.Numerics;
using OcpCore.Engine.Bitboards;
using Plane = OcpCore.Engine.Bitboards.Plane;

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
        
        // Exclude blockers
        mask &= ~game[colour];
                
        // Include opponents
        //mask &= game[opponentColour];

        // Remove blockers, account for opponents

        moves |= mask;

        mask = Moves[Kind.Rook][MoveSet.Vertical][position];
        
        // Remove blockers, account for opponents

        moves |= mask;
        
        return moves;
    }
}