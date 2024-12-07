using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Pawn : Piece
{
    public override int Value => Scores.Pawn;
    
    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moveSet = colour == Plane.White ? MoveSet.PawnToBlack : MoveSet.PawnToWhite;

        var moves = Moves[moveSet][position] & ~game[opponentColour] & ~game[colour];

        var rank = Cell.GetRank(position);

        switch (rank)
        {
            case Ranks.WhitePawnRank:
                moves &= ~((game[colour] & (Masks.ByteMask << Constants.BlackEnPassantTargetRankStart)) << Constants.Files);
        
                moves &= ~((game[opponentColour] & (Masks.ByteMask << Constants.BlackEnPassantTargetRankStart)) << Constants.Files);
                break;
            case Ranks.BlackPawnRank:
                moves &= ~((game[colour] & (Masks.ByteMask << Constants.WhiteEnPassantTargetRankStart)) >> Constants.Files);
        
                moves &= ~((game[opponentColour] & (Masks.ByteMask << Constants.WhiteEnPassantTargetRankStart)) >> Constants.Files);
                break;
        }

        var attackSet = colour == Plane.White ? MoveSet.PawnWhiteAttack : MoveSet.PawnBlackAttack;

        moves |= Moves[attackSet][position] & game[opponentColour];

        if (game.State.EnPassantTarget != null)
        {
            var target = game.State.EnPassantTarget.Value;
            
            // TODO: Magic numbers
            if (Cell.GetRank(position) == 4 && Cell.GetRank(target) == 5 && Math.Abs(position - target) is 7 or 9)
            {
                moves |= 1ul << target;
            } 
            else if (Cell.GetRank(position) == 3 && Cell.GetRank(target) == 2 && Math.Abs(position - target) is 7 or 9)
            {
                moves |= 1ul << target;
            }
        }

        return moves;
    }
}