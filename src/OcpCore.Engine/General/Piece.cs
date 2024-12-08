using System.Numerics;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Extensions;
using Plane = OcpCore.Engine.Bitboards.Plane;

namespace OcpCore.Engine.General;

public abstract class Piece
{
    protected static readonly Moves Moves = Moves.Instance;
    
    public abstract int Value { get; }

    public static int PopNextMove(ref ulong moves)
    {
        var emptyMoves = BitOperations.TrailingZeroCount(moves);

        if (emptyMoves == 64)
        {
            return -1;
        }

        moves ^= 1ul << emptyMoves;

        return emptyMoves;
    }

    public ulong GetMoves(Game game, int position)
    {
        var positionBit = 1ul << position;

        var colour = (game[Plane.White] & positionBit) == positionBit ? Plane.White : Plane.Black;

        return GetMoves(game, colour, colour.InvertColour(), position) & ~positionBit;
    }

    protected abstract ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position);

    public static ulong GetHorizontalSlidingMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var positionBit = 1ul << position;

        var mask = Moves[MoveSet.Horizontal][position];

        var rightBlockers = (game[colour] | game[opponentColour]) & mask & (~(positionBit - 1) - positionBit);

        var firstRightBlocker = rightBlockers != 0 ? BitOperations.TrailingZeroCount(rightBlockers) : 64;

        var rightMask = firstRightBlocker < 64 ? (1ul << firstRightBlocker) - 1 - positionBit : ulong.MaxValue;

        rightMask |= rightBlockers != 0 && (game[opponentColour] & (1ul << firstRightBlocker)) != 0
            ? 1ul << firstRightBlocker
            : 0;

        var leftBlockers = (game[colour] | game[opponentColour]) & mask & positionBit - 1;

        var firstLeftBlocker = leftBlockers != 0 ? BitOperations.LeadingZeroCount(leftBlockers) - 1 : 64;

        var leftMask = firstLeftBlocker < 64 ? ~((1ul << (63 - firstLeftBlocker)) - 1) : ulong.MaxValue;

        leftMask |= leftBlockers != 0 && (game[opponentColour] & (1ul << (63 - firstLeftBlocker - 1))) != 0
            ? 1ul << (63 - firstLeftBlocker - 1)
            : 0;
        
        return leftMask & rightMask & mask;
    }

    public static ulong GetVerticalSlidingMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var positionBit = 1ul << position;

        var mask = Moves[MoveSet.Vertical][position];

        var upBlockers = (game[colour] | game[opponentColour]) & mask & (~(positionBit - 1) - positionBit);

        var firstUpBlocker = upBlockers != 0 ? BitOperations.TrailingZeroCount(upBlockers) : 64;

        var upMask = firstUpBlocker < 64 ? (1ul << firstUpBlocker) - 1 - positionBit : ulong.MaxValue;

        upMask |= upBlockers != 0 && (game[opponentColour] & (1ul << firstUpBlocker)) != 0
            ? 1ul << firstUpBlocker
            : 0;

        var downBlockers = (game[colour] | game[opponentColour]) & mask & positionBit - 1;

        var firstDownBlocker = downBlockers != 0 ? BitOperations.LeadingZeroCount(downBlockers) - 1 : 64;

        var downMask = firstDownBlocker < 64 ? ~((1ul << (63 - firstDownBlocker)) - 1) : ulong.MaxValue;

        downMask |= downBlockers != 0 && (game[opponentColour] & (1ul << (63 - firstDownBlocker - 1))) != 0
            ? 1ul << (63 - firstDownBlocker - 1)
            : 0;

        return upMask & downMask & mask;
    }
    
    public static ulong GetDiagonalSlidingMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var positionBit = 1ul << position;

        var mask = Moves[MoveSet.Diagonal][position];

        var topRightBlockers = (game[colour] | game[opponentColour]) & mask & (~(positionBit - 1) - positionBit);

        var firstTopRightBlocker = topRightBlockers != 0 ? BitOperations.TrailingZeroCount(topRightBlockers) : 64;
        
        var topRightMask = firstTopRightBlocker < 64 ? (1ul << firstTopRightBlocker) - 1 - positionBit : ulong.MaxValue;

        topRightMask |= topRightBlockers != 0 && (game[opponentColour] & (1ul << firstTopRightBlocker)) != 0
            ? 1ul << firstTopRightBlocker
            : 0;
        
        var bottomLeftBlockers = (game[colour] | game[opponentColour]) & mask & positionBit - 1;

        var firstBottomLeftBlocker = bottomLeftBlockers != 0 ? BitOperations.LeadingZeroCount(bottomLeftBlockers) - 1 : 64;
        
        var bottomLeftMask = firstBottomLeftBlocker < 64 ? ~((1ul << (63 - firstBottomLeftBlocker)) - 1) : ulong.MaxValue;

        bottomLeftMask |= bottomLeftBlockers != 0 && (game[opponentColour] & (1ul << (63 - firstBottomLeftBlocker - 1))) != 0
            ? 1ul << (63 - firstBottomLeftBlocker - 1)
            : 0;

        return topRightMask & bottomLeftMask & mask;
    }
    
    public static ulong GetAntiDiagonalSlidingMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var positionBit = 1ul << position;

        var mask = Moves[MoveSet.AntiDiagonal][position];
        
        var topLeftBlockers = (game[colour] | game[opponentColour]) & mask & positionBit - 1;

        var firstTopLeftBlocker = topLeftBlockers != 0 ? BitOperations.LeadingZeroCount(topLeftBlockers) - 1 : 64;

        var topLeftMask = firstTopLeftBlocker < 64 ? ~((1ul << (63 - firstTopLeftBlocker)) - 1) : ulong.MaxValue;

        topLeftMask |= topLeftBlockers != 0 && (game[opponentColour] & (1ul << (63 - firstTopLeftBlocker - 1))) != 0
            ? 1ul << (63 - firstTopLeftBlocker - 1)
            : 0;
        
        var bottomRightBlockers = (game[colour] | game[opponentColour]) & mask & (~(positionBit - 1) - positionBit);

        var firstBottomRightBlocker = bottomRightBlockers != 0 ? BitOperations.TrailingZeroCount(bottomRightBlockers) : 64;

        var bottomRightMask = firstBottomRightBlocker < 64 ? (1ul << firstBottomRightBlocker) - 1 - positionBit : ulong.MaxValue;

        bottomRightMask |= bottomRightBlockers != 0 && (game[opponentColour] & (1ul << firstBottomRightBlocker)) != 0
            ? 1ul << firstBottomRightBlocker
            : 0;

        return topLeftMask & bottomRightMask & mask;
    }
}