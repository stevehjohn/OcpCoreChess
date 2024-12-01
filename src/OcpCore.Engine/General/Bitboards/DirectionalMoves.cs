using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.General.Bitboards;

public class DirectionalMoves
{
    private readonly ulong[][] _attack;

    public ulong[] this[MoveSet direction] => _attack[(int) direction];

    public DirectionalMoves()
    {
        var sets = Enum.GetValues<MoveSet>().Length;

        _attack = new ulong[sets + 1][];
        
        for (var i = 0; i < sets; i++)
        {
            _attack[i] = new ulong[Constants.Cells];
        }
    }
}