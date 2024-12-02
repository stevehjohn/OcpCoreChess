using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.Pieces;

public class Pawn : Piece
{
    public override Kind Kind => Kind.Pawn;
    
    public override int Value => 1;

    public Pawn(Moves moves) : base(moves)
    {
    }
    
    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        return 0;
    }
    
}