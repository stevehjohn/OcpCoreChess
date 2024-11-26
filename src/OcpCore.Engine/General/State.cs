using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public class State
{
    private readonly int _state;

    public Colour Player => (_state & Constants.PlayerTurnBit) > 0 ? Colour.Black : Colour.White;

    public int EnPassantTarget => _state >> Constants.EnPassantOffset;

    public Castle CastleStatus => (Castle) (_state & Constants.CastleStatusMask);

    public State(Colour player, int enPassantTarget, Castle castleStatus)
    {
        var state = 0;

        state |= player == Colour.White ? 0 : Constants.PlayerTurnBit;
        
        state |= enPassantTarget << Constants.EnPassantOffset;
        
        state |= (int) castleStatus;

        _state = state;
    }
}