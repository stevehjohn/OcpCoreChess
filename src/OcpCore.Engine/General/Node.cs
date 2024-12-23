using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.General;

public class Node
{
    private readonly Node _parent;

    private readonly bool _isMaximising;
    
    public Game Game { get; }

    public int Depth { get; }
    
    public int Root { get; }
    
    public int Score { get; private set; }

    public Node(Game game, int depth, int root, bool isMaximising)
    {
        Game = game;
        
        Depth = depth;
        
        Root = root;

        _isMaximising = isMaximising;
    }
    
    public Node(Node parent, Game game, int depth, int root, int score)
    {
        _parent = parent;
        
        Game = game;
        
        Depth = depth;
        
        Root = root;

        Score = score;

        _isMaximising = ! parent._isMaximising;
    }

    public void PropagateScore(int score)
    {
        var node = _parent;

        while (node != null)
        {
            if (_isMaximising)
            {
                if (score > node.Score)
                {
                    node.Score = score;
                }
            }
            else
            {
                if (score < node.Score)
                {
                    node.Score = score;
                }
            }

            node = node._parent;
        }
    }
}