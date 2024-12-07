using System.Numerics;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using Plane = OcpCore.Engine.Bitboards.Plane;

namespace OcpCore.Engine;

public sealed class Core : IDisposable
{
    public const string Name = "Ocp Core Chess";

    public const string Author = "Stevo John";

    private readonly Game _game;

    private readonly Colour _engineColour;

    private readonly PriorityQueue<(Game Game, int Depth, string PerftNode), int> _gameQueue = new();
    
    private long[] _depthCounts;
    
    private long[][] _outcomes;
    
    private readonly Dictionary<string, long> _perftCounts = new();

    public IReadOnlyDictionary<string, long> PerftCounts => _perftCounts;

    private CancellationTokenSource _cancellationTokenSource;

    private CancellationToken _cancellationToken;

    private Task _getMoveTask;

    public long GetDepthCount(int ply) => _depthCounts[ply];

    public long GetMoveOutcome(int ply, MoveOutcome outcome) => _outcomes[ply][BitOperations.Log2((byte) outcome) + 1];

    public bool IsBusy => _cancellationTokenSource != null;

    public int QueueSize { get; private set; }

    public Core(Colour engineColour)
    {
        _engineColour = engineColour;

        _game = new Game();
        
        _game.ParseFen(Constants.InitialBoardFen);
    }

    public Core(Colour engineColour, string fen)
    {
        _engineColour = engineColour;

        _game = new Game();
        
        _game.ParseFen(fen);
    }

    public void MakeMove(string move)
    {
        var position = move[..2].FromStandardNotation();
    
        var target = move[2..].FromStandardNotation();

        var positionBit = 1ul << position;

        if ((_game[(Plane) _game.State.Player] & positionBit) == 0)
        {
            throw new InvalidMoveException($"No piece at {move[..2]}.");
        }

        var kind = _game.GetKind(position);
        
        var moves = PieceCache.Get(kind).GetMoves(_game, position);

        if ((moves & positionBit) == 0)
        {
            throw new InvalidMoveException($"{move} is not a valid move for a {kind}.");
        }
    
        _game.MakeMove(position, target);
    }

    public void GetMove(int depth)
    {
        GetMoveInternal(depth);
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
    
    public List<string> GetAllowedMoves()
    {
        var allowedMoves = new List<string>();
    
        var player = _game.State.Player;
               
        var pieces = _game[(Plane) player];

        var cell = PopPiecePosition(ref pieces);

        while (cell > -1)
        {
            var kind = _game.GetKind(cell);

            var moves = PieceCache.Get(kind).GetMoves(_game, cell);

            var move = Piece.PopNextMove(ref moves);

            while (move > -1)
            {
                allowedMoves.Add($"{cell.ToStandardNotation()}{move.ToStandardNotation()}");

                move = Piece.PopNextMove(ref moves);
            }

            cell = PopPiecePosition(ref pieces);
        }

        return allowedMoves;
    }
    
    private void GetMoveInternal(int depth, Action callback = null)
    {
        _depthCounts = new long[depth + 1];

        _outcomes = new long[depth + 1][];
        
        _gameQueue.Clear();

        for (var i = 1; i <= depth; i++)
        {
            _depthCounts[i] = 0;

            _outcomes[i] = new long[Constants.MoveOutcomes + 1];
        }

        _gameQueue.Enqueue((_game, depth, null), 0);

        var threads = Environment.ProcessorCount - 2;
        
        var countdown = new CountdownEvent(threads);

        for (var i = 0; i < threads; i++)
        {
            Task.Run(() =>
            {
                var result = ProcessQueue(depth);

                for (var d = 1; d <= depth; d++)
                {
                    Interlocked.Add(ref _depthCounts[d], result.Counts[d]);

                    for (var o = 1; o < 8; o++)
                    {
                        Interlocked.Add(ref _outcomes[d][o], result.Outcomes[d][o]);
                    }
                }

                countdown.Signal();
            }, _cancellationToken);
        }

        while (! countdown.IsSet)
        {
            QueueSize = _gameQueue.Count;
            
            Thread.Sleep(1_000);
        }

        callback?.Invoke();
    }

    private (long[] Counts, long[][] Outcomes) ProcessQueue(int maxDepth)
    {
        var wait = 10;
        
        while (wait > 0 && _gameQueue.Count == 0)
        {
            Thread.Sleep(100);

            wait--;
        }

        var localCounts = new long[maxDepth + 1];

        var localOutcomes = new long[maxDepth + 1][];
        
        for (var i = 1; i <= maxDepth; i++)
        {
            localOutcomes[i] = new long[Constants.MoveOutcomes + 1];
        }

        var localQueue = new PriorityQueue<(Game Game, int Depth, string PerftNode), int>();

        while (_gameQueue.Count > 0)
        {
            lock (_gameQueue)
            {
                for (var i = 0; i < Math.Max(1, _gameQueue.Count / Environment.ProcessorCount); i++)
                {
                    if (_gameQueue.Count == 0)
                    {
                        break;
                    }

                    if (_gameQueue.TryDequeue(out var item, out var priority))
                    {
                        localQueue.Enqueue(item, priority);
                    }
                }
            }

            while (localQueue.Count > 0)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    return (localCounts, localOutcomes);
                }

                if (! localQueue.TryDequeue(out (Game, int, string) state, out _))
                {
                    return (localCounts, localOutcomes);
                }

                var (game, depth, perftNode) = state;

                var player = game.State.Player;

                var ply = maxDepth - depth + 1;

                var pieces = game[(Plane) player];

                var cell = PopPiecePosition(ref pieces);

                while (cell > -1)
                {
                    var kind = game.GetKind(cell);

                    var moves = PieceCache.Get(kind).GetMoves(game, cell);

                    var move = Piece.PopNextMove(ref moves);

                    while (move > -1)
                    {
                        var copy = new Game(game);

                        var outcomes = copy.MakeMove(cell, move);

                        if (copy.IsKingInCheck((Plane) player))
                        {
                            move = Piece.PopNextMove(ref moves);

                            continue;
                        }

                        localCounts[ply]++;

                        if (localCounts[ply] > 1_000)
                        {
                            Interlocked.Add(ref _depthCounts[ply], localCounts[ply]);

                            localCounts[ply] = 0;
                        }

                        if (copy.IsKingInCheck((Plane) player.Invert()))
                        {
                            outcomes |= MoveOutcome.Check;

                            if (! CanMove(copy, player.Invert()))
                            {
                                outcomes |= MoveOutcome.CheckMate;
                            }
                        }

                        while (outcomes > 0)
                        {
                            var outcome = BitOperations.TrailingZeroCount((int) outcomes);

                            localOutcomes[ply][outcome + 1]++;

                            outcomes ^= (MoveOutcome) (1 << outcome);
                        }

                        if (perftNode == null)
                        {
                            perftNode = $"{cell.ToStandardNotation()}{move.ToStandardNotation()}";

                            _perftCounts.Add(perftNode, 1);
                        }
                        else
                        {
                            _perftCounts[perftNode]++;
                        }

                        if (depth > 1)
                        {
                            if (_gameQueue.Count < 100)
                            {
                                lock (_gameQueue)
                                {
                                    _gameQueue.Enqueue((copy, depth - 1, perftNode), MoveOutcome.CheckMate - outcomes);

                                    _perftCounts[perftNode]--;
                                }
                            }
                            else
                            {
                                localQueue.Enqueue((copy, depth - 1, perftNode), MoveOutcome.CheckMate - outcomes);

                                _perftCounts[perftNode]--;
                            }
                        }
                    
                        if (depth == maxDepth)
                        {
                            perftNode = null;
                        }

                        move = Piece.PopNextMove(ref moves);
                    }

                    cell = PopPiecePosition(ref pieces);
                }
            }
        }

        return (localCounts, localOutcomes);
    }
    
    private static bool CanMove(Game game, Colour colour)
    {
        var pieces = game[(Plane) colour];

        var cell = PopPiecePosition(ref pieces);

        while (cell > -1)
        {
            var kind = game.GetKind(cell);

            var moves = PieceCache.Get(kind).GetMoves(game, cell);

            var move = Piece.PopNextMove(ref moves);

            while (move > -1)
            {
                var copy = new Game(game);

                copy.MakeMove(cell, move);

                if (copy.IsKingInCheck((Plane) colour))
                {
                    move = Piece.PopNextMove(ref moves);
                
                    continue;
                }

                return true;
            }

            cell = PopPiecePosition(ref pieces);
        }

        return false;
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
        
        _getMoveTask?.Dispose();
    }
}