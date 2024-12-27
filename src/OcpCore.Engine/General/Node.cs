using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.General;

public class Node
{
    private int _score;

    private Node _parent;

    private bool _isMaximising;
    
    public Game Game { get; private set; }

    public int Depth { get; private set; }
    
    public int Root { get; private set; }
    
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
    }

    private void PropagateScore(int score)
    {
        _score = score;
    }
}