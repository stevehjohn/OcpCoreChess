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
        var moveSet = colour == Plane.White ? MoveSet.PawnToBlack : MoveSet.PawnToWhite;

        var moves = Moves[Kind.Pawn][moveSet][position] & ~game[opponentColour];

        var attackSet = colour == Plane.White ? MoveSet.PawnWhiteAttack : MoveSet.PawnBlackAttack;

        moves |= Moves[Kind.Pawn][attackSet][position] & game[opponentColour];

        return moves;
    }
}