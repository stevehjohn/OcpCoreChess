using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.Pieces;

public class Knight : Piece
{
    public override Kind Kind => Kind.Knight;
    
    public override int Value => 3;

    public Knight(Moves moves) : base(moves)
    {
    }
    
    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        return Moves[Kind.Knight][MoveSet.Specific][position];
    }
}