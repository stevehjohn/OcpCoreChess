namespace OcpCore.Engine.General;

[Flags]
public enum MoveOutcome
{
    Move      = 1,
    Capture   = 2,
    EnPassant = 4,
    Castle    = 8,
    Promotion = 16,
    Check     = 32,
    CheckMate = 64
}