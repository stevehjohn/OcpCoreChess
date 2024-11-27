using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public class State
{
    private int _state;

    public Colour Player => (_state & Masks.PlayerTurn) > 0 ? Colour.Black : Colour.White;

    public Castle CastleStatus => (Castle) (_state & Masks.CastleStatus);

    public int? EnPassantTarget => (_state & Masks.EnPassantTarget) == Masks.EnPassantTarget ? null : (_state & Masks.EnPassantTarget) >> Offsets.EnPassantTargetOffset;

    public int WhiteScore => (_state >> Offsets.WhiteScoreOffset) & Masks.ByteMask;

    public int BlackScore => (_state >> Offsets.BlackScoreOffset) & Masks.ByteMask;
    
    public State(Colour player, Castle castleStatus, int? enPassantTarget, int whiteScore, int blackScore)
    {
        var state = 0;

        state |= player == Colour.White ? 0 : Masks.PlayerTurn;

        state |= enPassantTarget == null ? Masks.EnPassantTarget : (enPassantTarget.Value & Masks.EnPassantBits) << Offsets.EnPassantTargetOffset;
        
        state |= (int) castleStatus;

        state |= (whiteScore & Masks.ByteMask) << Offsets.WhiteScoreOffset;

        state |= (blackScore & Masks.ByteMask) << Offsets.BlackScoreOffset;

        _state = state;
    }

    public void RemoveCastleRights(Castle castle)
    {
        _state &= ~(int) castle;
    }

    public void SetEnPassantTarget(int? target)
    {
        if (target == null)
        {
            _state |= Masks.EnPassantTarget;
        }
        else
        {
            _state &= ~Masks.EnPassantTarget;
            
            _state |= (target.Value & Masks.EnPassantBits) << Offsets.EnPassantTargetOffset;
        }
    }
}