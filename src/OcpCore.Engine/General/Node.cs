using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Extensions;

namespace OcpCore.Engine.General;

public class Node
{
    private int _score;

    private readonly Node _parent;

    public Game Game { get; private set; }

    public int Depth { get; private set; }
    
    public int Root { get; set; }
    
    public int Alpha { get; set; } = int.MinValue;

    public int Beta { get; set; } = int.MaxValue;

    public bool IsMaximising { get; }

    public string Move => $"{(Root >> 8).ToStandardNotation()}{(Root & 0xFF).ToStandardNotation()}";

    public Node(Game game, int depth, bool isMaximising)
    {
        Game = game;
        
        Depth = depth;

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

    public void PropagateScore(int score)
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