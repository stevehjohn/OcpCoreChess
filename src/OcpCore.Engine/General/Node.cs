using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.General;

public class Node
{
    private int _score;

    private readonly Node _parent;

    public Game Game { get; private set; }

    public int Depth { get; private set; }
    
    public int Root { get; private set; }
    
    public int Alpha { get; private set; } = int.MinValue;

    public int Beta { get; private set; } = int.MaxValue;

    public bool IsMaximising { get; }

    public int Score
    {
        set => PropagateScore(value);
    }

    public Node(Game game, int depth, int root, bool isMaximising)
    {
        Game = game;
        
        Depth = depth;
        
        Root = root;

        IsMaximising = isMaximising;
    }
    
    public Node(Node parent, Game game, int depth, int root)
    {
        _parent = parent;

        IsMaximising = ! _parent.IsMaximising;
        
        Game = game;
        
        Depth = depth;
        
        Root = root;

        Alpha = _parent.Alpha;

        Beta = _parent.Beta;
    }

    private void PropagateScore(int score)
    {
        _score = score;

        var node = _parent;

        while (node != null)
        {
            node._score = node.IsMaximising
                ? Math.Max(node._score, _score)
                : Math.Min(node._score, _score);

            if (node.IsMaximising)
            {
                node.Alpha = Math.Max(node.Alpha, score);
            }
            else
            {
                node.Beta = Math.Min(node.Beta, score);
            }

            if (node.Alpha >= node.Beta)
            {
                break;
            }

            node = node._parent;
        }
    }
}