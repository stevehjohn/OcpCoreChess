using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.General;

public class State
{
    private ulong _state;

    public Colour Player => (_state & Masks.PlayerTurn) > 0 ? Colour.Black : Colour.White;

    public Castle CastleStatus => (Castle) (_state & Masks.CastleStatus);

    public int? EnPassantTarget => (_state & Masks.EnPassantTarget) == Masks.EnPassantTarget ? null : (int?) ((_state & Masks.EnPassantTarget) >> Offsets.EnPassantTarget);

    public int WhiteScore => (int) ((_state >> Offsets.WhiteScore) & Masks.ByteMask);

    public int BlackScore => (int) ((_state >> Offsets.BlackScore) & Masks.ByteMask);

    public int WhiteKingCell => (int) ((_state >> Offsets.WhiteKing) & Masks.PositionBits);

    public int BlackKingCell => (int) ((_state >> Offsets.BlackKing) & Masks.PositionBits);

    public int Halfmoves => (int) ((_state >> Offsets.Halfmove) & Masks.ByteMask);

    public int Fullmoves => (int) ((_state >> Offsets.Fullmove) & Masks.Fullmove);

    public State()
    {
        SetState(Colour.White, Castle.WhiteQueenSide | Castle.WhiteKingSide | Castle.BlackQueenSide | Castle.BlackKingSide, 0, 0, 0, 0, 0, 0, 1);
    }
    
    public State(Colour player, Castle castleStatus, int? enPassantTarget, int whiteScore, int blackScore, int whiteKingCell, int blackKingCell, int halfmoves, int fullmoves)
    {
        SetState(player, castleStatus, enPassantTarget, whiteScore, blackScore, whiteKingCell, blackKingCell, halfmoves, fullmoves);
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
            
            _state |= ((ulong) target.Value & Masks.EnPassantBits) << Offsets.EnPassantTarget;
        }
    }

    public void UpdateWhiteScore(int delta)
    {
        var score = WhiteScore;
        
        _state &= ~(Masks.ByteMask << Offsets.WhiteScore);

        _state |= ((ulong) (score + delta) & Masks.ByteMask) << Offsets.WhiteScore;
    }
    
    public void UpdateBlackScore(int delta)
    {
        var score = BlackScore;
        
        _state &= ~(Masks.ByteMask << Offsets.BlackScore);

        _state |= ((ulong) (score + delta) & Masks.ByteMask) << Offsets.BlackScore;
    }

    public void SetWhiteKingCell(int cell)
    {
        _state &= ~(Masks.PositionBits << Offsets.WhiteKing);

        _state |= ((ulong) cell & Masks.PositionBits) << Offsets.WhiteKing;
    }

    public void SetBlackKingCell(int cell)
    {
        _state &= ~(Masks.PositionBits << Offsets.BlackKing);

        _state |= ((ulong) cell & Masks.PositionBits) << Offsets.BlackKing;
    }

    public void IncrementHalfmoves()
    {
        var halfmoves = Halfmoves;
        
        _state &= ~(Masks.ByteMask << Offsets.Halfmove);
        
        _state |= (((ulong) halfmoves + 1) & Masks.ByteMask) << Offsets.Halfmove;
    }
    
    public void ResetHalfmoves()
    {
        _state &= ~(Masks.ByteMask << Offsets.Halfmove);
    }

    public void IncrementFullmoves()
    {
        var fullmoves = Fullmoves;
        
        _state &= ~(Masks.ByteMask << Offsets.Fullmove);
        
        _state |= (((ulong) fullmoves + 1) & Masks.Fullmove) << Offsets.Fullmove;
    }

    public void InvertPlayer()
    {
        _state ^= Masks.PlayerTurn;
    }

    private void SetState(Colour player, Castle castleStatus, int? enPassantTarget, int whiteScore, int blackScore, int whiteKingCell, int blackKingCell, int halfmoves, int fullmoves)
    {
        var state = 0ul;

        state |= player == Colour.White ? 0 : Masks.PlayerTurn;

        state |= enPassantTarget == null ? Masks.EnPassantTarget : ((ulong) enPassantTarget.Value & Masks.EnPassantBits) << Offsets.EnPassantTarget;
        
        state |= (ulong) castleStatus;

        state |= ((ulong) whiteScore & Masks.ByteMask) << Offsets.WhiteScore;

        state |= ((ulong) blackScore & Masks.ByteMask) << Offsets.BlackScore;

        state |= ((ulong) whiteKingCell & Masks.PositionBits) << Offsets.WhiteKing;

        state |= ((ulong) blackKingCell & Masks.PositionBits) << Offsets.BlackKing;

        state |= ((ulong) halfmoves & Masks.ByteMask) << Offsets.Halfmove;
        
        state |= ((ulong) fullmoves & Masks.ByteMask) << Offsets.Fullmove;

        _state = state;
    }
}