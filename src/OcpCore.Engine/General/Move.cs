namespace OcpCore.Engine.General;

public struct Move
{
    public int NewPosition { get; }
    
    public bool Captures { get; }

    public Move(int newPosition, bool captures)
    {
        NewPosition = newPosition;
        
        Captures = captures;
    }
}