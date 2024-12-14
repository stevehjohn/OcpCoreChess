namespace OcpCore.Engine.General;

[Flags]
public enum MoveOutcome
{
    Move      = 1,
    Castle    = 2,
    Promotion = 4,
    EnPassant = 8,
    Capture   = 16,
    Check     = 32,
    CheckMate = 64
}