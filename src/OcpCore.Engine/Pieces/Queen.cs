using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.Pieces;

public class Queen : Piece
{
    public override Kind Kind => Kind.Queen;
    
    public override int Value => 9;

    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moves = GetHorizontalSlidingMoves(game, colour, opponentColour, position);
        
        moves |= GetVerticalSlidingMoves(game, colour, opponentColour, position);

        moves |= GetDiagonalSlidingMoves(game, colour, opponentColour, position);

        moves |= GetAntiDiagonalSlidingMoves(game, colour, opponentColour, position);
        
        return moves;
    }
}