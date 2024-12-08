using System.Numerics;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Kernel;

public class StateProcessor
{
    private const int CentralPoolMax = 1_000;
    
    private readonly PriorityQueue<(Game game, int depth), int> _centralQueue;
    
    private readonly PriorityQueue<(Game game, int depth), int> _localQueue = new();

    private int _maxDepth;

    private long[] _depthCounts;

    private long[][] _outcomes;

    private Action<StateProcessor, bool> _callback;

    public long GetDepthCount(int ply) => _depthCounts[ply];

    public long GetOutcomeCount(int ply, MoveOutcome outcome) => _outcomes[ply][BitOperations.Log2((byte) outcome)];

    public StateProcessor(PriorityQueue<(Game game, int depth), int> centralQueue)
    {
        _centralQueue = centralQueue;
    }

    public void StartProcessing(int maxDepth, Action<StateProcessor, bool> callback, CancellationToken cancellationToken)
    {
        _maxDepth = maxDepth;

        _callback = callback;
        
        _depthCounts = new long[maxDepth + 1];

        _outcomes = new long[maxDepth + 1][];

        for (var i = 1; i <= _maxDepth; i++)
        {
            _depthCounts[i] = 0;

            _outcomes[i] = new long[Constants.MoveOutcomes + 1];
        }
        
        // ReSharper disable once InconsistentlySynchronizedField
        while (_centralQueue.Count > 0)
        {
            lock (_centralQueue)
            {
                for (var i = 0; i < Math.Max(1, _centralQueue.Count / Coordinator.Threads); i++)
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
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var (game, depth) = _localQueue.Dequeue();
                
                ProcessWorkItem(game, depth);
            }
        }

        callback(this, true);
    }

    private void ProcessWorkItem(Game game, int depth)
    {
        var player = game.State.Player;

        var ply = _maxDepth - depth + 1;

        var pieces = game[player];

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

                if (copy.IsKingInCheck(player))
                {
                    move = Piece.PopNextMove(ref moves);

                    continue;
                }

                IncrementCounts(ply);

                if (copy.IsKingInCheck(player.Invert()))
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
        var pieces = game[colour];

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

                if (copy.IsKingInCheck(colour))
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
            _callback(this, false);

            for (var i = 0; i <= _maxDepth; i++)
            {
                _depthCounts[i] = 0;
            }
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
        if (_centralQueue.Count < CentralPoolMax)
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