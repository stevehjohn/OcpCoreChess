using System.Numerics;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using Plane = OcpCore.Engine.Bitboards.Plane;

namespace OcpCore.Engine.Kernel;

public class StateProcessor
{
    private static readonly int Threads = Environment.ProcessorCount - 2;
    
    private readonly int _maxDepth;

    private readonly PriorityQueue<(Game game, int depth), int> _centralQueue;
    
    private readonly PriorityQueue<(Game game, int depth), int> _localQueue = new();

    private readonly CancellationToken _cancellationToken;

    private readonly long[] _centralDepthCounts;
    
    private readonly long[] _depthCounts;
    
    private readonly long[][] _outcomes;

    public StateProcessor(int maxDepth, PriorityQueue<(Game game, int depth), int> centralQueue, long[] centralDepthCounts, CancellationToken cancellationToken)
    {
        _maxDepth = maxDepth;

        _centralQueue = centralQueue;

        _centralDepthCounts = centralDepthCounts;

        _cancellationToken = cancellationToken;
        
        _depthCounts = new long[maxDepth + 1];

        _outcomes = new long[maxDepth + 1][];

        for (var i = 1; i <= maxDepth; i++)
        {
            _depthCounts[i] = 0;

            _outcomes[i] = new long[Constants.MoveOutcomes + 1];
        }
    }

    public void StartProcessing()
    {
        // ReSharper disable once InconsistentlySynchronizedField
        while (_centralQueue.Count > 0)
        {
            lock (_centralQueue)
            {
                for (var i = 0; i < Math.Max(1, _centralQueue.Count / Threads); i++)
                {
                    if (_centralQueue.TryDequeue(out var workItem, out var priority))
                    {
                        _localQueue.Enqueue(workItem, priority);
                        
                        continue;
                    }
                    
                    break;
                }
            }

            while (_localQueue.Count > 0)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var (game, depth) = _localQueue.Dequeue();
                
                ProcessWorkItem(game, depth);
            }
        }
    }

    private void ProcessWorkItem(Game game, int depth)
    {
        var player = game.State.Player;

        var ply = _maxDepth - depth + 1;

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

                IncrementCounts(ply);

                if (copy.IsKingInCheck((Plane) player.Invert()))
                {
                    outcomes |= MoveOutcome.Check;

                    if (! CanMove(copy, player.Invert()))
                    {
                        outcomes |= MoveOutcome.CheckMate;
                    }
                }

                IncrementOutcomes(ply, outcomes);

                if (depth > 1 && (outcomes & MoveOutcome.CheckMate) == 0)
                {
                    Enqueue(copy, depth - 1, MoveOutcome.CheckMate - outcomes);
                }

                move = Piece.PopNextMove(ref moves);
            }

            cell = PopPiecePosition(ref pieces);
        }
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
    
    private void IncrementCounts(int ply)
    {
        _depthCounts[ply]++;

        if (_depthCounts[ply] > 1_000)
        {
            Interlocked.Add(ref _centralDepthCounts[ply], _depthCounts[ply]);
        
            _depthCounts[ply] = 0;
        }
    }

    private void IncrementOutcomes(int ply, MoveOutcome outcomes)
    {
        while (outcomes > 0)
        {
            var outcome = BitOperations.TrailingZeroCount((int) outcomes);

            _outcomes[ply][outcome + 1]++;

            outcomes ^= (MoveOutcome) (1 << outcome);
        }
    }

    private void Enqueue(Game game, int depth, int priority)
    {
        // ReSharper disable once InconsistentlySynchronizedField - Doesn't need to be exactly 1,000.
        if (_centralQueue.Count < 1_000)
        {
            lock (_centralQueue)
            {
                _centralQueue.Enqueue((game, depth), priority);
            }
        }
        else
        {
            _localQueue.Enqueue((game, depth), priority);
        }
    }
}