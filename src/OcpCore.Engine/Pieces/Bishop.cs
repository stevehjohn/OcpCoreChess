using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.Pieces;

public class Bishop : Piece
{
    public override Kind Kind => Kind.Bishop;
    
    public override int Value => 3;

    public Bishop(Moves moves) : base(moves)
    {
    }

    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moves = GetDiagonalSlidingMoves(game, colour, opponentColour, position);
        
        moves |= GetAntiDiagonalSlidingMoves(game, colour, opponentColour, position);

        return moves;
    }
}