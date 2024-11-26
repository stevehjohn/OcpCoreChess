using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public class State
{
    private readonly int _state;

    public Colour Player => (_state & Masks.PlayerTurn) > 0 ? Colour.Black : Colour.White;

    public Castle CastleStatus => (Castle) (_state & Masks.CastleStatus);

    public int? EnPassantTarget => _state >> Constants.EnPassantOffset == 255 ? null : _state >> Constants.EnPassantOffset;

    public State(Colour player, Castle castleStatus, int? enPassantTarget)
    {
        var state = 0;

        state |= player == Colour.White ? 0 : Masks.PlayerTurn;
        
        state |= (enPassantTarget ?? 255) << Constants.EnPassantOffset;
        
        state |= (int) castleStatus;

        _state = state;
    }

    public void RemoveCastleRights(Castle castle)
    {
    }
}