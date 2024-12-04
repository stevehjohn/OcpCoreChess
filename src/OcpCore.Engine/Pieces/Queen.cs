using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Queen : Piece
{
    public override int Value => Scores.Queen;

    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moves = GetHorizontalSlidingMoves(game, colour, opponentColour, position);
        
        moves |= GetVerticalSlidingMoves(game, colour, opponentColour, position);

        moves |= GetDiagonalSlidingMoves(game, colour, opponentColour, position);

        moves |= GetAntiDiagonalSlidingMoves(game, colour, opponentColour, position);
        
        return moves;
    }
}