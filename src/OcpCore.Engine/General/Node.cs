using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.General;

public struct Node
{
    public Game Game { get; private set; }

    public int Depth { get; private set; }
    
    public int Root { get; private set; }

    public Node(Game game, int depth, int root)
    {
        Game = game;
        
        Depth = depth;
        
        Root = root;
    }
}