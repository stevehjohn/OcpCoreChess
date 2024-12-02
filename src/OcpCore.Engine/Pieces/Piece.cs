using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Extensions;

namespace OcpCore.Engine.Pieces;

public abstract class Piece
{
    public abstract Kind Kind { get; }
    
    public abstract int Value { get; }

    protected Moves Moves;

    protected Piece(Moves moves)
    {
        Moves = moves;
    }
    
    public ulong GetMoves(Game game, int position)
    {
        var positionBit = 1ul << position;

        var colour = (game[Plane.White] & positionBit) == positionBit ? Plane.White : Plane.Black;

        return GetMoves(game, colour, colour.InvertColour(), position) & ~positionBit;
    }

    protected abstract ulong GetMoves(Game game, Plane colour, Plane opponentColour, int positionBit);
}