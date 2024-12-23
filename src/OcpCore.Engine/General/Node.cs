using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.General;

public class Node
{
    public Node Parent { get; }
    
    public Game Game { get; }

    public int Depth { get; }
    
    public int Root { get; }
    
    public int Score { get; }

    public Node(Game game, int depth, int root)
    {
        Game = game;
        
        Depth = depth;
        
        Root = root;
    }
    
    public Node(Node parent, Game game, int depth, int root, int score)
    {
        Parent = parent;
        
        Game = game;
        
        Depth = depth;
        
        Root = root;

        Score = score;
    }
}