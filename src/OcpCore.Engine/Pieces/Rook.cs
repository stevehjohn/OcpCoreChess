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
        var moves = GetHorizontalSlidingMoves(game, colour, opponentColour, position);

        // Include opponents
        //mask &= game[opponentColour];
        
        moves |= GetVerticalSlidingMoves(game, colour, opponentColour, position);
        
        return moves;
    }

    private ulong GetHorizontalSlidingMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var positionBit = 1ul << position;

        var mask = Moves[Kind.Rook][MoveSet.Horizontal][position];
        
        var rightBlockers = game[colour] & mask & (~(positionBit - 1) - positionBit);

        var firstRightBlocker = rightBlockers != 0 ? BitOperations.TrailingZeroCount(rightBlockers) : 64;

        var rightMask = firstRightBlocker < 64 ? (1ul << firstRightBlocker) - 1 - positionBit : ulong.MaxValue;

        var leftBlockers = game[colour] & mask & positionBit - 1;
        
        var firstLeftBlocker = leftBlockers != 0 ? BitOperations.LeadingZeroCount(leftBlockers) - 1 : 64;
        
        var leftMask = firstLeftBlocker < 64 ? ~((1ul << (63 - firstLeftBlocker)) - 1 ): ulong.MaxValue;

        return leftMask & rightMask & mask;
    }

    private ulong GetVerticalSlidingMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var positionBit = 1ul << position;

        var mask = Moves[Kind.Rook][MoveSet.Vertical][position];

        var rightBlockers = game[colour] & mask & (~(positionBit - 1) - positionBit);

        var firstRightBlocker = rightBlockers != 0 ? BitOperations.TrailingZeroCount(rightBlockers) : 64;

        var rightMask = firstRightBlocker < 64 ? (1ul << firstRightBlocker) - 1 - positionBit : ulong.MaxValue;

        var leftBlockers = game[colour] & mask & positionBit - 1;
        
        var firstLeftBlocker = leftBlockers != 0 ? BitOperations.LeadingZeroCount(leftBlockers) - 1 : 64;
        
        var leftMask = firstLeftBlocker < 64 ? ~((1ul << (63 - firstLeftBlocker)) - 1 ): ulong.MaxValue;

        return leftMask & rightMask & mask;
    }
}