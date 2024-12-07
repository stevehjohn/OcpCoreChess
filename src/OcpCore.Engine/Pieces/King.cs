using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class King : Piece
{
    public override int Value => Scores.King;

    protected override ulong GetMoves(Game game, Plane colour, Plane opponentColour, int position)
    {
        var moves = Moves[MoveSet.King][position];

        moves &= ~game[colour];

        if (CheckCanCastleKingSide(game, colour, position))
        {
            moves |= 1ul << (position + 2);
        }

        if (CheckCanCastleQueenSide(game, colour, position))
        {
            moves |= 1ul << (position - 2);
        }

        return moves;
    }

    private static bool CheckCanCastleKingSide(Game game, Plane colour, int position)
    {
        if (colour == Plane.White)
        {
            if ((game.State.CastleStatus & Castle.WhiteKingSide) == 0)
            {
                return false;
            }
        }
        else
        {
            if ((game.State.CastleStatus & Castle.BlackKingSide) == 0)
            {
                return false;
            }
        }

        if (game.IsKingInCheck(colour))
        {
            return false;
        }

        if (game.IsEmpty(position + 1) && game.IsEmpty(position + 2))
        {
            if (game.IsKingInCheck(colour, position + 1))
            {
                return false;
            }

            return true;
        }

        return false;
    }
    
    private static bool CheckCanCastleQueenSide(Game game, Plane colour, int position)
    {
        if (colour == Plane.White)
        {
            if ((game.State.CastleStatus & Castle.WhiteQueenSide) == 0)
            {
                return false;
            }
        }
        else
        {
            if ((game.State.CastleStatus & Castle.BlackQueenSide) == 0)
            {
                return false;
            }
        }

        if (game.IsKingInCheck(colour))
        {
            return false;
        }

        if (game.IsEmpty(position - 1) && game.IsEmpty(position - 2) && game.IsEmpty(position - 3))
        {
            if (game.IsKingInCheck(colour, position - 1))
            {
                return false;
            }

            return true;
        }

        return false;
    }
}