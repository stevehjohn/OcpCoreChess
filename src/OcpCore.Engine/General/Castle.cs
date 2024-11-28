namespace OcpCore.Engine.General;

[Flags]
public enum Castle
{
    NotAvailable   = 0,
    WhiteQueenSide = 1,
    WhiteKingSide  = 2,
    White          = 3,
    BlackQueenSide = 4,
    BlackKingSide  = 8,
    Black          = 12,
    All            = 15
}