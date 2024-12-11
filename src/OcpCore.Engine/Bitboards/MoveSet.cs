namespace OcpCore.Engine.Bitboards;

public class MoveSet
{
    private readonly ulong[] _moves;

    public ulong this[MoveSets moveSet]
    {
        get => _moves[(int) moveSet];
        set => _moves[(int) moveSet] = value;
    }

    public MoveSet()
    {
        var sets = Enum.GetValues<MoveSets>().Length;

        _moves = new ulong[sets];
    }
}