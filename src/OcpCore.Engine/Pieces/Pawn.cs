using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Pawn : Piece
{
    public override int Value => Scores.Pawn;
    
    protected override ulong GetMoves(Game game, Colour colour, Colour opponentColour, int position)
    {
        var moveSet = colour == Colour.White ? MoveSets.PawnToBlack : MoveSets.PawnToWhite;

        var moves = Moves[position][moveSet] & ~game[opponentColour] & ~game[colour];

        var rank = Cell.GetRank(position);

        switch (rank)
        {
            case Ranks.WhitePawn:
                moves &= ~((game[colour] & (Masks.Byte << Constants.BlackEnPassantTargetRankStart)) << Constants.Files);
        
                moves &= ~((game[opponentColour] & (Masks.Byte << Constants.BlackEnPassantTargetRankStart)) << Constants.Files);
                break;
            case Ranks.BlackPawn:
                moves &= ~((game[colour] & (Masks.Byte << Constants.WhiteEnPassantTargetRankStart)) >> Constants.Files);
        
                moves &= ~((game[opponentColour] & (Masks.Byte << Constants.WhiteEnPassantTargetRankStart)) >> Constants.Files);
                break;
        }

        var attackSet = colour == Colour.White ? MoveSets.PawnWhiteAttack : MoveSets.PawnBlackAttack;

        moves |= Moves[position][attackSet] & game[opponentColour];

        if (game.State.EnPassantTarget != null)
        {
            var target = game.State.EnPassantTarget.Value;
            
            if ((Cell.GetRank(position) == Ranks.WhiteEnPassantTarget - 1 && Cell.GetRank(target) == Ranks.WhiteEnPassantTarget && Math.Abs(position - target) is 7 or 9)
                || (Cell.GetRank(position) == Ranks.BlackEnPassantTarget + 1 && Cell.GetRank(target) == Ranks.BlackEnPassantTarget && Math.Abs(position - target) is 7 or 9))
            {
                moves |= 1ul << target;
            } 
        }

        return moves;
    }
}