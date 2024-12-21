namespace OcpCore.Engine.General;

[Flags]
public enum MoveOutcome
{
    Move      = 1,
    Castle    = 2,
    EnPassant = 4,
    Capture   = 8,
    Check     = 16,
    Promotion = 32,
    CheckMate = 64
}