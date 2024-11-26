namespace OcpCore.Engine.General;

[Flags]
public enum Castle
{
    NotAvailable = 0,
    WhiteQueenSide = 1,
    WhiteKingSide  = 2,
    BlackQueenSide = 4,
    BlackKingSide  = 8
}