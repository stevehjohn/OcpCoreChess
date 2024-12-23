using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.General;

public class Node
{
    private readonly Node _parent;

    public Game Game { get; }

    public int Depth { get; }
    
    public int Root { get; }
    
    public int Score { get; private set; }

    public bool IsMaximising { get; }

    public Node(Game game, int depth, int root, bool isMaximising)
    {
        Game = game;
        
        Depth = depth;
        
        Root = root;

        IsMaximising = isMaximising;

        Score = isMaximising ? int.MinValue : int.MaxValue;
    }
    
    public Node(Node parent, Game game, int depth, int root, int score)
    {
        _parent = parent;
        
        Game = game;
        
        Depth = depth;
        
        Root = root;

        Score = score;

        IsMaximising = ! parent.IsMaximising;
    }

    public void PropagateScore(int score)
    {
        var node = this;
        
        while (node != null)
        {
            if (node.IsMaximising)
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