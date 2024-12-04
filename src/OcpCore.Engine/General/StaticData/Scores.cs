namespace OcpCore.Engine.General.StaticData;

public static class Scores
{
    public const int King = 0;
    public const int Pawn = 1;
    public const int Rook = 5;
    public const int Knight = 3;
    public const int Bishop = 3;
    public const int Queen = 9;

    public const int Initial = Pawn * 8 + Rook * 2 + Knight * 2 + Bishop * 2 + Queen;
}