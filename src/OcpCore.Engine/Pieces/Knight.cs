using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Pieces;

public class Knight : Piece
{
    public override int Value => Scores.Knight;
    
    protected override ulong GetMoves(Game game, Colour colour, Colour opponentColour, int position)
    {
        var moves = Moves[position][MoveSets.Knight];

        moves &= ~game[colour];
        
        return moves;
    }
}