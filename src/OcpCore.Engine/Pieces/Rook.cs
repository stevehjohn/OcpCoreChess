using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;
using Plane = OcpCore.Engine.Bitboards.Plane;

namespace OcpCore.Engine.Pieces;

public class Rook : Piece
{
    public override Kind Kind => Kind.Rook;
    
    public override int Value => Scores.Rook;

    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moves = GetHorizontalSlidingMoves(game, colour, opponentColour, position);
        
        moves |= GetVerticalSlidingMoves(game, colour, opponentColour, position);
        
        return moves;
    }
}