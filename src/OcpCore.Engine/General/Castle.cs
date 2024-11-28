namespace OcpCore.Engine.General;

[Flags]
public enum Castle
{
    NotAvailable   = 0,
    WhiteKingSide  = 1,
    WhiteQueenSide = 2,
    White          = 3,
    BlackKingSide  = 4,
    BlackQueenSide = 8,
    Black          = 12,
    All            = 15
}