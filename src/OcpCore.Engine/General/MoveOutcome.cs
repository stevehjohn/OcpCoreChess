namespace OcpCore.Engine.General;

[Flags]
public enum MoveOutcome
{
    Move      = 1,
    Castle    = 2,
    Capture   = 4,
    EnPassant = 8,
    Promotion = 16,
    Check     = 32,
    CheckMate = 64
}