using System.Numerics;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Extensions;

namespace OcpCore.Engine.General;

public abstract class Piece
{
    protected static readonly MoveCache Moves = MoveCache.Instance;
    
    public abstract int Value { get; }

    public ulong GetMoves(Game game, int position)
    {
        var positionBit = 1ul << position;

        var colour = (game[Colour.White] & positionBit) == positionBit ? Colour.White : Colour.Black;

        return GetMoves(game, colour, colour.Invert(), position) & ~positionBit;
    }

    protected abstract ulong GetMoves(Game game, Colour colour, Colour opponentColour, int position);

    public static ulong GetHorizontalSlidingMoves(Game game, Colour colour, Colour opponentColour, int position)
    {
        var positionBit = 1ul << position;

        var mask = Moves[position][MoveSets.Horizontal];

        var gameOpponentColour = game[opponentColour];

        var gameColour = game[colour];

        var rightBlockers = (gameColour | gameOpponentColour) & mask & (~(positionBit - 1) - positionBit);

        var firstRightBlocker = rightBlockers != 0 ? BitOperations.TrailingZeroCount(rightBlockers) : 64;

        var rightMask = firstRightBlocker < 64 ? (1ul << firstRightBlocker) - 1 - positionBit : ulong.MaxValue;

        rightMask |= rightBlockers != 0 && (gameOpponentColour & (1ul << firstRightBlocker)) != 0
            ? 1ul << firstRightBlocker
            : 0;

        var leftBlockers = (gameColour | gameOpponentColour) & mask & positionBit - 1;

        var firstLeftBlocker = leftBlockers != 0 ? BitOperations.LeadingZeroCount(leftBlockers) - 1 : 64;

        var leftMask = firstLeftBlocker < 64 ? ~((1ul << (63 - firstLeftBlocker)) - 1) : ulong.MaxValue;

        leftMask |= leftBlockers != 0 && (gameOpponentColour & (1ul << (63 - firstLeftBlocker - 1))) != 0
            ? 1ul << (63 - firstLeftBlocker - 1)
            : 0;
        
        return leftMask & rightMask & mask;
    }

    public static ulong GetVerticalSlidingMoves(Game game, Colour colour, Colour opponentColour, int position)
    {
        var positionBit = 1ul << position;

        var mask = Moves[position][MoveSets.Vertical];

        var gameOpponentColour = game[opponentColour];

        var gameColour = game[colour];

        var upBlockers = (gameColour | gameOpponentColour) & mask & (~(positionBit - 1) - positionBit);

        var firstUpBlocker = upBlockers != 0 ? BitOperations.TrailingZeroCount(upBlockers) : 64;

        var upMask = firstUpBlocker < 64 ? (1ul << firstUpBlocker) - 1 - positionBit : ulong.MaxValue;

        upMask |= upBlockers != 0 && (gameOpponentColour & (1ul << firstUpBlocker)) != 0
            ? 1ul << firstUpBlocker
            : 0;

        var downBlockers = (gameColour | gameOpponentColour) & mask & positionBit - 1;

        var firstDownBlocker = downBlockers != 0 ? BitOperations.LeadingZeroCount(downBlockers) - 1 : 64;

        var downMask = firstDownBlocker < 64 ? ~((1ul << (63 - firstDownBlocker)) - 1) : ulong.MaxValue;

        downMask |= downBlockers != 0 && (gameOpponentColour & (1ul << (63 - firstDownBlocker - 1))) != 0
            ? 1ul << (63 - firstDownBlocker - 1)
            : 0;

        return upMask & downMask & mask;
    }
    
    public static ulong GetDiagonalSlidingMoves(Game game, Colour colour, Colour opponentColour, int position)
    {
        var positionBit = 1ul << position;

        var mask = Moves[position][MoveSets.Diagonal];

        var gameOpponentColour = game[opponentColour];

        var gameColour = game[colour];

        var topRightBlockers = (gameColour | gameOpponentColour) & mask & (~(positionBit - 1) - positionBit);

        var firstTopRightBlocker = topRightBlockers != 0 ? BitOperations.TrailingZeroCount(topRightBlockers) : 64;
        
        var topRightMask = firstTopRightBlocker < 64 ? (1ul << firstTopRightBlocker) - 1 - positionBit : ulong.MaxValue;

        topRightMask |= topRightBlockers != 0 && (gameOpponentColour & (1ul << firstTopRightBlocker)) != 0
            ? 1ul << firstTopRightBlocker
            : 0;
        
        var bottomLeftBlockers = (gameColour | gameOpponentColour) & mask & positionBit - 1;

        var firstBottomLeftBlocker = bottomLeftBlockers != 0 ? BitOperations.LeadingZeroCount(bottomLeftBlockers) - 1 : 64;
        
        var bottomLeftMask = firstBottomLeftBlocker < 64 ? ~((1ul << (63 - firstBottomLeftBlocker)) - 1) : ulong.MaxValue;

        bottomLeftMask |= bottomLeftBlockers != 0 && (gameOpponentColour & (1ul << (63 - firstBottomLeftBlocker - 1))) != 0
            ? 1ul << (63 - firstBottomLeftBlocker - 1)
            : 0;

        return topRightMask & bottomLeftMask & mask;
    }
    
    public static ulong GetAntiDiagonalSlidingMoves(Game game, Colour colour, Colour opponentColour, int position)
    {
        var positionBit = 1ul << position;

        var mask = Moves[position][MoveSets.AntiDiagonal];
        
        var gameOpponentColour = game[opponentColour];

        var gameColour = game[colour];

        var topLeftBlockers = (gameColour | gameOpponentColour) & mask & positionBit - 1;

        var firstTopLeftBlocker = topLeftBlockers != 0 ? BitOperations.LeadingZeroCount(topLeftBlockers) - 1 : 64;

        var topLeftMask = firstTopLeftBlocker < 64 ? ~((1ul << (63 - firstTopLeftBlocker)) - 1) : ulong.MaxValue;

        topLeftMask |= topLeftBlockers != 0 && (gameOpponentColour & (1ul << (63 - firstTopLeftBlocker - 1))) != 0
            ? 1ul << (63 - firstTopLeftBlocker - 1)
            : 0;
        
        var bottomRightBlockers = (gameColour | gameOpponentColour) & mask & (~(positionBit - 1) - positionBit);

        var firstBottomRightBlocker = bottomRightBlockers != 0 ? BitOperations.TrailingZeroCount(bottomRightBlockers) : 64;

        var bottomRightMask = firstBottomRightBlocker < 64 ? (1ul << firstBottomRightBlocker) - 1 - positionBit : ulong.MaxValue;

        bottomRightMask |= bottomRightBlockers != 0 && (gameOpponentColour & (1ul << firstBottomRightBlocker)) != 0
            ? 1ul << firstBottomRightBlocker
            : 0;

        return topLeftMask & bottomRightMask & mask;
    }
}