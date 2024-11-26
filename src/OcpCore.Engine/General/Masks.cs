namespace OcpCore.Engine.General;

public static class Masks
{
    public const byte File = 0b0000_0111;
    public const byte Colour = 0b0000_1000;
    public const int CastleStatus = 0b0000_1111;
    public const int PlayerTurn = 0b0001_0000;
    public const int NoEnPassant = 0b0111_1111;
}