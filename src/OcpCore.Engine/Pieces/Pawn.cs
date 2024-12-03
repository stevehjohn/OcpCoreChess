using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Pawn : Piece
{
    public override Kind Kind => Kind.Pawn;
    
    public override int Value => 1;
    
    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moveSet = colour == Plane.White ? MoveSet.PawnToBlack : MoveSet.PawnToWhite;

        var moves = Moves[Kind.Pawn][moveSet][position] & ~game[opponentColour] & ~game[colour];

        var rank = Cell.GetRank(position);

        // TODO: Magic numbers
        switch (rank)
        {
            case Ranks.WhitePawnRank:
                moves &= ~((game[colour] & (Masks.ByteMask << 16)) << 8);

                moves &= ~((game[opponentColour] & (Masks.ByteMask << 16)) << 8);
                break;
            case Ranks.BlackPawnRank:
                moves &= ~((game[colour] & (Masks.ByteMask << 40)) >> 8);

                moves &= ~((game[opponentColour] & (Masks.ByteMask << 40)) >> 8);
                break;
        }

        var attackSet = colour == Plane.White ? MoveSet.PawnWhiteAttack : MoveSet.PawnBlackAttack;

        moves |= Moves[Kind.Pawn][attackSet][position] & game[opponentColour];

        return moves;
    }
}