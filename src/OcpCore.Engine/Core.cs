using System.Numerics;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Kernel;

namespace OcpCore.Engine;

public sealed class Core : IDisposable
{
    public const string Name = "Ocp Core Chess";

    public const string Author = "Stevo John";

    private readonly Colour _engineColour;

    private readonly PieceCache _pieceCache = PieceCache.Instance;
    
    private readonly PerfTestCollector _perfTestCollector;

    private Game _game;

    private Coordinator _coordinator;

    private CancellationTokenSource _cancellationTokenSource;

    private CancellationToken _cancellationToken;

    private Task<string> _getMoveTask;

    public long GetDepthCount(int ply) => _coordinator.GetDepthCount(ply);

    public long GetOutcomeCount(int ply, MoveOutcome outcome) => _coordinator.GetOutcomeCount(ply, outcome);

    public bool IsBusy => _cancellationTokenSource != null;

    public int QueueSize => _coordinator?.QueueSize ?? 0;

    public IReadOnlyDictionary<string, long> PerftData => _perfTestCollector?.Counts;

    public Core(Colour engineColour, bool collectPerft = false)
    {
        _engineColour = engineColour;

        if (collectPerft)
        {
            _perfTestCollector = new PerfTestCollector();
        }

        _game = new Game();
        
        _game.ParseFen(Constants.InitialBoardFen);
    }

    public Core(Colour engineColour, string fen, bool collectPerft = false)
    {
        _engineColour = engineColour;

        if (collectPerft)
        {
            _perfTestCollector = new PerfTestCollector();
        }

        _game = new Game();
        
        _game.ParseFen(fen);
    }

    public void MakeMove(string move)
    {
        var position = move[..2].FromStandardNotation();
    
        var target = move[2..].FromStandardNotation();

        var kind = _game.GetKind(position);
        
        var moves = _pieceCache[kind].GetMoves(_game, position);
        
        if ((moves & (1ul << target)) == 0)
        {
            throw new InvalidMoveException($"{move} is not a valid move for a {kind}.");
        }
    
        _game.MakeMove(position, target);
    }

    public string GetMove(int depth)
    {
        return GetMoveInternal(depth);
    }
    
    public Task<string> GetMove(int depth, Action callback)
    {
        _cancellationTokenSource = new CancellationTokenSource();

        _cancellationToken = _cancellationTokenSource.Token;

        _getMoveTask = Task.Run(() =>
        {
            var bestMove = GetMoveInternal(depth, callback);

            _cancellationTokenSource = null;

            _getMoveTask = null;
            
            return bestMove;

        }, _cancellationToken);

        return _getMoveTask;
    }
    
    public List<string> GetAllowedMoves()
    {
        var allowedMoves = new List<string>();
    
        var player = _game.State.Player;
               
        var pieces = _game[player];

        var cell = PopPiecePosition(ref pieces);

        while (cell > -1)
        {
            var kind = _game.GetKind(cell);

            var moves = _pieceCache[kind].GetMoves(_game, cell);

            var move = moves.PopBit();

            while (move > -1)
            {
                var copy = new Game(_game);

                copy.MakeMove(cell, move);

                if (! copy.IsKingInCheck(player))
                {
                    allowedMoves.Add($"{cell.ToStandardNotation()}{move.ToStandardNotation()}");
                }
                
                move = moves.PopBit();
            }

            cell = PopPiecePosition(ref pieces);
        }

        return allowedMoves;
    }

    private string GetMoveInternal(int depth, Action callback = null)
    {
        _coordinator = new Coordinator(_engineColour, _perfTestCollector);
        
        _coordinator.StartProcessing(_game, depth);

        callback?.Invoke();

        return "";
    }
    
    private static int PopPiecePosition(ref ulong pieces)
    {
        var emptyMoves = BitOperations.TrailingZeroCount(pieces);

        if (emptyMoves == 64)
        {
            return -1;
        }

        pieces ^= 1ul << emptyMoves;

        return emptyMoves;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();

        _cancellationTokenSource = null;
        
        _getMoveTask?.Dispose();

        _getMoveTask = null;
        
        _coordinator?.Dispose();

        _coordinator = null;
    }
}