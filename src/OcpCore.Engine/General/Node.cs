namespace OcpCore.Engine.General;

public class Node
{
    private readonly List<Node> _children = [];
    
    public Move Move { get; }

    public int Score { get; }

    public IReadOnlyList<Node> Children => _children;

    public Node()
    {
    }
    
    public Node(Move move, int score)
    {
        Move = move;
        
        Score = score;
    }

    public void AddChild(Move move, int score)
    {
        _children.Add(new Node(move, score));
    }
}