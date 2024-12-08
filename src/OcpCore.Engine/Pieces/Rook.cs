using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Rook : Piece
{
    public override int Value => Scores.Rook;

    protected override ulong GetMoves(Game game, Colour colour, Colour opponentColour, int position)
    {
        var moves = GetHorizontalSlidingMoves(game, colour, opponentColour, position);
        
        moves |= GetVerticalSlidingMoves(game, colour, opponentColour, position);
        
        return moves;
    }
}