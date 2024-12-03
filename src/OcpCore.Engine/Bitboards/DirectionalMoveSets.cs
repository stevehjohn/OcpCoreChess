using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Bitboards;

public class DirectionalMoveSets
{
    private readonly ulong[][] _attack;

    public ulong[] this[MoveSet direction] => _attack[(int) direction];

    public DirectionalMoveSets()
    {
        var sets = Enum.GetValues<MoveSet>().Length;

        _attack = new ulong[sets + 1][];
        
        for (var i = 0; i < sets; i++)
        {
            _attack[i] = new ulong[Constants.Cells];
        }
    }
}