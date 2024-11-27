using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine;

public class Core
{
    public const string Name = "Ocp Core Chess";

    public const string Author = "Stevo John";

    private const int DefaultDepth = 6;

    private readonly Board _board;

    private readonly int _defaultDepth;

    public Core(int defaultDepth = DefaultDepth)
    {
        _board = new Board(Constants.InitialBoardFen);

        _defaultDepth = defaultDepth;
    }

    public Core(string fen, int defaultDepth = DefaultDepth)
    {
        _board = new Board(fen);

        _defaultDepth = defaultDepth;
    }

    public void MakeMove(string move)
    {
        var position = move[..2].FromStandardNotation();

        var target = move[2..].FromStandardNotation();
        
        _board.MakeMove(position, target);
    }

    public void GetMove()
    {
        GetMove(_defaultDepth);
    }

    public void GetMove(int depth)
    {
        ProcessPly(_board);
    }

    private void ProcessPly(Board board)
    {
        var moves = new List<Move>();
        
        GetAllMoves(board, moves);
        
        // TODO: This is where move ordering could be applied

        for (var i = 0; i < moves.Count; i++)
        {
            var move = moves[i];

            var copy = new Board(board);
            
            copy.MakeMove(move.Position, move.Target);
        }
    }

    private static void GetAllMoves(Board board, List<Move> moves)
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            var piece = board[cell];

            if (piece == 0)
            {
                continue;
            }

            if (Cell.Colour(piece) != board.State.Player)
            {
                continue;
            }
            
            PieceCache.Get(piece).GetMoves(board, cell, board.State.Player, moves);
        }
    }
}