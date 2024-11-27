using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public class State
{
    private ulong _state;

    public Colour Player => (_state & Masks.PlayerTurn) > 0 ? Colour.Black : Colour.White;

    public Castle CastleStatus => (Castle) (_state & Masks.CastleStatus);

    public int? EnPassantTarget => (_state & Masks.EnPassantTarget) == Masks.EnPassantTarget ? null : (int?) ((_state & Masks.EnPassantTarget) >> Offsets.EnPassantTargetOffset);

    public int WhiteScore => (int) ((_state >> Offsets.WhiteScoreOffset) & Masks.ByteMask);

    public int BlackScore => (int) ((_state >> Offsets.BlackScoreOffset) & Masks.ByteMask);

    public int WhiteKingPosition => (int) ((_state >> Offsets.WhiteKingOffset) & Masks.PositionBits);

    public int BlackKingPosition => (int) ((_state >> Offsets.BlackKingOffset) & Masks.PositionBits);
    
    public State(Colour player, Castle castleStatus, int? enPassantTarget, int whiteScore, int blackScore, int whiteKingCell, int blackKingCell)
    {
        var state = 0ul;

        state |= player == Colour.White ? 0 : Masks.PlayerTurn;

        state |= (ulong) (enPassantTarget == null ? Masks.EnPassantTarget : (enPassantTarget.Value & Masks.EnPassantBits) << Offsets.EnPassantTargetOffset);
        
        state |= (ulong) castleStatus;

        state |= (ulong) ((whiteScore & Masks.ByteMask) << Offsets.WhiteScoreOffset);

        state |= (ulong) ((blackScore & Masks.ByteMask) << Offsets.BlackScoreOffset);

        state |= ((ulong) whiteKingCell & Masks.PositionBits) << Offsets.WhiteKingOffset;

        state |= ((ulong) blackKingCell & Masks.PositionBits) << Offsets.BlackKingOffset;

        _state = state;
    }

    public State(State state)
    {
        _state = state._state;
    }

    public void RemoveCastleRights(Castle castle)
    {
        _state &= ~(ulong) castle;
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
            
            _state |= (ulong) ((target.Value & Masks.EnPassantBits) << Offsets.EnPassantTargetOffset);
        }
    }

    public void UpdateWhiteScore(int delta)
    {
        var score = WhiteScore;
        
        _state &= ~(Masks.ByteMask << Offsets.WhiteScoreOffset);

        _state |= (ulong) (((score + delta) & Masks.ByteMask) << Offsets.WhiteScoreOffset);
    }
    
    public void UpdateBlackScore(int delta)
    {
        var score = BlackScore;
        
        _state &= ~(Masks.PositionBits << Offsets.BlackScoreOffset);

        _state |= ((ulong) (score + delta) & Masks.PositionBits) << Offsets.BlackScoreOffset;
    }

    public void SetWhiteKingPosition(int position)
    {
        _state &= ~(Masks.PositionBits << Offsets.WhiteKingOffset);

        _state |= ((ulong) position & Masks.PositionBits) << Offsets.WhiteKingOffset;
    }

    public void SetBlackKingPosition(int position)
    {
        _state &= ~(Masks.PositionBits << Offsets.BlackKingOffset);

        _state |= ((ulong) position & Masks.PositionBits) << Offsets.BlackKingOffset;
    }

    public void InvertPlayer()
    {
        _state ^= Masks.PlayerTurn;
    }
}