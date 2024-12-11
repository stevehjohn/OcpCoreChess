namespace OcpCore.Engine.Bitboards;

public class MoveSet
{
    private Moves _moves;

    public ulong this[MoveSets moveSet]
    {
        get => _moves[(int) moveSet];
        set => _moves[(int) moveSet] = value;
    }
}