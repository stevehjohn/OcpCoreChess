using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine;

public class Core
{
    public const string Name = "Ocp Core Chess";

    public const string Author = "Stevo John";

    private const int DefaultDepth = 6;

    private readonly Board _board;

    private readonly int _defaultDepth;

    private readonly Colour _engineColour;

    public Core(Colour engineColour, int defaultDepth = DefaultDepth)
    {
        _engineColour = engineColour;
        
        _board = new Board(Constants.InitialBoardFen);

        _defaultDepth = defaultDepth;
    }

    public Core(Colour engineColour, string fen, int defaultDepth = DefaultDepth)
    {
        _engineColour = engineColour;
        
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
        ProcessPly(_board, depth, depth);
    }

    private void ProcessPly(Board board, int maxDepth, int depth)
    {
        var moves = new List<Move>();
        
        GetAllMoves(board, moves);
        
        // TODO: This is where move ordering could be applied

        var player = board.State.Player;
               
        var ply = maxDepth - depth + 1;

        for (var i = 0; i < moves.Count; i++)
        {
            var move = moves[i];

            var copy = new Board(board);

            copy.MakeMove(move.Position, move.Target);

            if (copy.IsKingInCheck(player))
            {
                continue;
            }

            if (copy.IsKingInCheck(player.Invert()))
            {
            }

            if (depth > 1)
            {
                ProcessPly(copy, maxDepth, depth - 1);
            }
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