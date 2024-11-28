namespace OcpCore.Engine.General;

public struct Move
{
    public int Position { get; }
    
    public int Target { get; }
    
    public MoveOutcome Outcome { get; }

    public Move(int position, int target, MoveOutcome outcome)
    {
        Position = position;
        
        Target = target;
        
        Outcome = outcome;
    }
}