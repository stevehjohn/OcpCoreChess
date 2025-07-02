namespace OcpCore.Engine.General;

public enum MoveOutcome
{
    EngineInCheckmate   = 0,
    Move                = 1,
    Stalemate           = 2,
    OpponentInCheckmate = 3
}