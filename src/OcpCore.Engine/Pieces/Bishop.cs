using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Bishop : Piece
{
    public override int Value => Scores.Bishop;

    protected override ulong GetMoves(Game game, Colour colour, Colour opponentColour, int position)
    {
        var moves = GetDiagonalSlidingMoves(game, colour, opponentColour, position);
        
        moves |= GetAntiDiagonalSlidingMoves(game, colour, opponentColour, position);

        return moves;
    }
}