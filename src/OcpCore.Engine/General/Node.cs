using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.General;

public struct Node
{
    public Game Game { get; }

    public int Depth { get; }
    
    public int Root { get; }
    
    public Move Move { get; }

    public Node(Game game, int depth, int root)
    {
        Game = game;
        
        Depth = depth;
        
        Root = root;
        
        Move = Move.Null;
    }

    public Node(Game game, int depth, int root, Move move)
    {
        Game = game;
        
        Depth = depth;
        
        Root = root;

        Move = move;
    }
}