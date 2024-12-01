using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.General.Bitboards;

public class DirectionalAttacks
{
    private readonly ulong[][] _attack = new ulong[Constants.Directions][];

    public ulong[] this[Direction direction] => _attack[(int) direction];

    public DirectionalAttacks()
    {
        for (var i = 0; i < Constants.Directions; i++)
        {
            _attack[i] = new ulong[Constants.Cells];
        }
    }
}