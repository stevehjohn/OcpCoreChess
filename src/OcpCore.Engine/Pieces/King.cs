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
        var kingColour = colour == Plane.White ? Colour.White : Colour.Black;

        if (kingColour == Colour.White)
        {
            if ((game.State.CastleStatus & Castle.White) == 0)
            {
                return false;
            }
        }
        else
        {
            if ((game.State.CastleStatus & Castle.Black) == 0)
            {
                return false;
            }
        }

        if (game.IsKingInCheck(kingColour))
        {
            return false;
        }

        if (game.IsEmpty(position + 1) && game.IsEmpty(position + 2))
        {
            if (game.IsKingInCheck(kingColour, position + 1))
            {
                return false;
            }

            if (game.IsKingInCheck(kingColour, position + 2))
            {
                return false;
            }

            return true;
        }

        return false;
    }
    
    private static bool CheckCanCastleQueenSide(Game game, Plane colour, int position)
    {
        var kingColour = colour == Plane.White ? Colour.White : Colour.Black;

        if (kingColour == Colour.White)
        {
            if ((game.State.CastleStatus & Castle.White) == 0)
            {
                return false;
            }
        }
        else
        {
            if ((game.State.CastleStatus & Castle.Black) == 0)
            {
                return false;
            }
        }

        if (game.IsKingInCheck(kingColour))
        {
            return false;
        }

        if (game.IsEmpty(position - 1) && game.IsEmpty(position - 2) && game.IsEmpty(position - 3))
        {
            if (game.IsKingInCheck(kingColour, position - 1))
            {
                return false;
            }

            if (game.IsKingInCheck(kingColour, position - 2))
            {
                return false;
            }

            if (game.IsKingInCheck(kingColour, position - 3))
            {
                return false;
            }

            return true;
        }

        return false;
    }
}