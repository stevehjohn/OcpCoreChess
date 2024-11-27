namespace OcpCore.Engine.General;

public struct Move
{
    public int Position { get; }
    
    public int Target { get; }
    
    public bool Captures { get; }

    public Move(int position, int target, bool captures)
    {
        Position = position;
        
        Target = target;
        
        Captures = captures;
    }
}