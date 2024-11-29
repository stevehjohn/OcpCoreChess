using OcpCore.Engine.Extensions;

namespace OcpCore.Engine.General;

public struct Move : IComparable<Move>
{
    public int Position { get; }
    
    public int Target { get; }
    
    public MoveOutcome Outcome { get; }

    public int Score { get; }

    public Move(int position, int target, MoveOutcome outcome, int score)
    {
        Position = position;
        
        Target = target;
        
        Outcome = outcome;

        Score = score;
    }

    public int CompareTo(Move other)
    {
        return other.Score - Score;
    }

    public override string ToString()
    {
        return $"{Position.ToStandardNotation()}{Target.ToStandardNotation()} {Outcome} {Score}";
    }
}