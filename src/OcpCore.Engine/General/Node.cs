using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.General;

public class Node
{
    private readonly Node _parent;

    private readonly string _move;
    
    public Game Game { get; }

    public int Depth { get; }
    
    public int Root { get; }
    
    public int Score { get; private set; }

    public bool IsMaximising { get; }
    
    public string Moves { get; private set; }

    public Node(Game game, int depth, int root, bool isMaximising)
    {
        Game = game;
        
        Depth = depth;
        
        Root = root;

        IsMaximising = isMaximising;

        if (isMaximising)
        {
            Score = int.MinValue;
        }
        else
        {
            Score = int.MaxValue;
        }
    }
    
    public Node(Node parent, Game game, int depth, int root, string move, int score)
    {
        _parent = parent;
        
        Game = game;
        
        Depth = depth;
        
        Root = root;

        _move = move;
        
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