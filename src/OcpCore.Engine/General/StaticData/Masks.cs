namespace OcpCore.Engine.General.StaticData;

public static class Masks
{
    public const byte File = 0b0000_0111;
    public const byte Kind = 0b0000_0111;
    public const byte Colour = 0b0000_1000;
    public const uint CastleStatus = 0b0000_1111;
    public const uint EnPassantTarget = 0b0000_1111_1110_0000;
    public const uint EnPassantBits = 0b0111_1111;
    public const uint PlayerTurn = 0b0001_0000;
    public const uint ByteMask = 0b1111_1111;
    public const ulong PositionBits = 0b0011_1111;
}