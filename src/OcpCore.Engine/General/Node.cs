using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.General;

public class Node
{
    private int _score;

    private readonly Node _parent;

    private readonly bool _isMaximising;
    
    public Game Game { get; private set; }

    public int Depth { get; private set; }
    
    public int Root { get; private set; }
    
    public int Alpha { get; set; } = int.MinValue;

    public int Beta { get; set; } = int.MaxValue;
    
    public int Score
    {
        get => _score;
        set => PropagateScore(value);
    }

    public Node(Game game, int depth, int root, bool isMaximising)
    {
        Game = game;
        
        Depth = depth;
        
        Root = root;

        _isMaximising = isMaximising;
    }
    
    public Node(Node parent, Game game, int depth, int root)
    {
        _parent = parent;

        _isMaximising = ! _parent._isMaximising;
        
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
            node._score = node._isMaximising
                ? Math.Max(node._score, _score)
                : Math.Min(node._score, _score);

            if (node._isMaximising)
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