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
    
    private Node(Move move, int score)
    {
        Move = move;
        
        Score = score;
    }

    public Node AddChild(Move move, int score)
    {
        var node = new Node(move, score);
        
        _children.Add(node);

        return node;
    }
}