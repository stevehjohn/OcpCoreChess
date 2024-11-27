using System.Numerics;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine;

public class Core : IDisposable
{
    public const string Name = "Ocp Core Chess";

    public const string Author = "Stevo John";

    private const int DefaultDepth = 6;

    private readonly Board _board;

    private readonly int _defaultDepth;

    private readonly Colour _engineColour;

    private long[] _depthCounts;
    
    private long[][] _outcomes;

    private CancellationTokenSource _cancellationTokenSource;

    private CancellationToken _cancellationToken;

    private Task _getMoveTask;

    public long GetDepthCount(int ply) => _depthCounts[ply];

    public long GetMoveOutcome(int ply, MoveOutcome outcome) => _outcomes[ply][BitOperations.Log2((byte) outcome) + 1];

    public bool IsBusy => _cancellationTokenSource != null;

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

    public void GetMove(int depth = 0)
    {
        if (depth == 0)
        {
            depth = _defaultDepth;
        }

        GetMoveInternal(_defaultDepth);
    }
    
    public Task GetMove(int depth, Action callback)
    {
        _cancellationTokenSource = new CancellationTokenSource();

        _cancellationToken = _cancellationTokenSource.Token;

        _getMoveTask = Task.Run(() =>
        {
            GetMoveInternal(depth, callback);

            _cancellationTokenSource = null;

            _getMoveTask = null;
            
        }, _cancellationToken);

        return _getMoveTask;
    }
    
    public void GetMoveInternal(int depth, Action callback = null)
    {
        _depthCounts = new long[depth + 1];

        _outcomes = new long[depth + 1][];
        
        for (var i = 1; i <= depth; i++)
        {
            _depthCounts[i] = 0;

            _outcomes[i] = new long[Constants.MoveOutcomes + 1];
        }

        ProcessPly(_board, depth, depth);

        callback?.Invoke();
    }

    private void ProcessPly(Board board, int maxDepth, int depth)
    {
        if (_cancellationToken.IsCancellationRequested)
        {
            return;
        }   var moves = new List<Move>();
        
        GetAllMoves(board, moves);
        
        // TODO: This is where move ordering could be applied

        var player = board.State.Player;
               
        var ply = maxDepth - depth + 1;

        for (var i = 0; i < moves.Count; i++)
        {
            var move = moves[i];

            var copy = new Board(board);

            var outcome = copy.MakeMove(move.Position, move.Target);

            if (copy.IsKingInCheck(player))
            {
                continue;
            }

            _depthCounts[ply]++;

            if (copy.IsKingInCheck(player.Invert()))
            {
                outcome |= MoveOutcome.Check;
                
                if (! OpponentCanMove(copy, player.Invert()))
                {
                    outcome |= MoveOutcome.CheckMate;
                }
            }

            for (var j = 0; j <= Constants.MoveOutcomes; j++)
            {
                if (((byte) outcome & (1 << j)) > 0)
                {
                    _outcomes[ply][j + 1]++;
                }
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

    private static bool OpponentCanMove(Board board, Colour colour)
    {
        var moves = new List<Move>();
        
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            var piece = board[cell];
            
            if (piece == 0)
            {
                continue;
            }

            if (Cell.Colour(piece) != colour)
            {
                continue;
            }

            PieceCache.Get(piece).GetMoves(board, cell, colour, moves);

            foreach (var move in moves)
            {
                var copy = new Board(board);

                copy.MakeMove(cell, move.Target);

                if (copy.IsKingInCheck(colour))
                {
                    continue;
                }

                return true;
            }
        }

        return false;
    }
    
    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
        
        _getMoveTask?.Dispose();
    }
}