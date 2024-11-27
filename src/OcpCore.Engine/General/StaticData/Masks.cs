namespace OcpCore.Engine.General.StaticData;

public static class Masks
{
    public const byte File = 0b0000_0111;
    public const byte Kind = 0b0000_0111;
    public const byte Colour = 0b0000_1000;
    public const int CastleStatus = 0b0000_1111;
    public const int EnPassantTarget = 0b0000_1111_1110_0000;
    public const int EnPassantBits = 0b0111_1111;
    public const int PlayerTurn = 0b0001_0000;
    public const int ByteMask = 0b1111_1111;
}