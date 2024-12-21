namespace OcpCore.Engine.General;

public readonly struct Move : IEquatable<Move>
{
    public Kind Kind { get; }
    
    public int From { get; }
    
    public int To { get; }

    public Move(Kind kind, int from, int to)
    {
        Kind = kind;
        
        From = from;
        
        To = to;
    }

    public void Deconstruct(out Kind kind, out int from, out int to)
    {
        kind = Kind;

        from = From;

        to = To;
    }

    public static readonly Move Null = new(0, -1, -1);

    public static bool operator ==(Move left, Move right) => left.Kind == right.Kind && left.From == right.From && left.To == right.To;

    public static bool operator !=(Move left, Move right) => ! (left == right);

    public bool Equals(Move other)
    {
        return Kind == other.Kind && From == other.From && To == other.To;
        
    }

    public override bool Equals(object obj)
    {
        return obj is Move other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int) Kind, From, To);
    }
}