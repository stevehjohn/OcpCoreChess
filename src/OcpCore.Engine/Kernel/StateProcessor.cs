using System.Numerics;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Kernel;

public class StateProcessor
{
    private const int CentralPoolMax = 1_000;
    
    private readonly PriorityQueue<(Game game, int depth, int root), int> _centralQueue;
    
    private readonly PriorityQueue<(Game game, int depth, int root), int> _localQueue = new();
    
    private readonly PieceCache _pieceCache = PieceCache.Instance;

    private readonly PerftCollector _perftCollector;
    
    private int _maxDepth;

    private long[] _depthCounts;

    private long[][] _outcomes;

    private Action<StateProcessor, bool> _callback;

    public long GetDepthCount(int ply) => _depthCounts[ply];

    public long GetOutcomeCount(int ply, MoveOutcome outcome) => _outcomes[ply][BitOperations.Log2((byte) outcome)];

    public StateProcessor(PriorityQueue<(Game game, int depth, int root), int> centralQueue, PerftCollector perftCollector = null)
    {
        _centralQueue = centralQueue;

        _perftCollector = perftCollector;
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

                var (game, depth, root) = _localQueue.Dequeue();
                
                ProcessWorkItem(game, depth, root);
            }
        }

        callback(this, true);
    }

    private void ProcessWorkItem(Game game, int depth, int root)
    {
        var player = game.State.Player;

        var ply = _maxDepth - depth + 1;

        var pieces = game[player];

        var cell = pieces.PopBit();

        while (cell > -1)
        {
            var kind = game.GetKind(cell);

            var moves = _pieceCache[kind].GetMoves(game, cell);

            var move = moves.PopBit();

            while (move > -1)
            {
                var copy = new Game(game);

                var outcomes = copy.MakeMove(cell, move);

                if (copy.IsKingInCheck(player))
                {
                    move = moves.PopBit();

                    continue;
                }

                var opponent = player.Invert();

                var promotionResult = HandlePromotion(ref outcomes, copy, root, move, depth, opponent);
                
                if (promotionResult.Promoted)
                {
                    _depthCounts[ply] += 4;

                    if (_perftCollector != null)
                    {
                        if (ply == 1)
                        {
                            root = cell << 8 | move;
                        }

                        _perftCollector.AddCount(ply, root, 4);
                    }

                    if ((outcomes & MoveOutcome.Capture) > 0)
                    {
                        _outcomes[ply][BitOperations.TrailingZeroCount((int) MoveOutcome.Capture) + 1] += 4;
                    }

                    _outcomes[ply][BitOperations.TrailingZeroCount((int) MoveOutcome.Promotion) + 1] += 4;
                
                    _outcomes[ply][BitOperations.TrailingZeroCount((int) MoveOutcome.Check) + 1] += promotionResult.Checks;

                    _outcomes[ply][BitOperations.TrailingZeroCount((int) MoveOutcome.CheckMate) + 1] += promotionResult.CheckMates;
                
                    move = moves.PopBit();
                
                    continue;
                }

                IncrementCounts(ply);

                if (_perftCollector != null)
                {
                    if (ply == 1)
                    {
                        root = cell << 8 | move;
                    }

                    _perftCollector.AddCount(ply, root);
                }

                if (copy.IsKingInCheck(opponent))
                {
                    outcomes |= MoveOutcome.Check;

                    if (! CanMove(copy, opponent))
                    {
                        outcomes |= MoveOutcome.CheckMate;
                    }
                }

                IncrementOutcomes(ply, outcomes);

                if (depth > 1 && (outcomes & (MoveOutcome.CheckMate | MoveOutcome.Promotion)) == 0)
                {
                    Enqueue(copy, depth - 1, root, CalculatePriority(game, outcomes, move, kind, opponent));
                }

                move = moves.PopBit();
            }

            cell = pieces.PopBit();
        }
    }

    private (bool Promoted, int Checks, int CheckMates) HandlePromotion(ref MoveOutcome outcomes, Game game, int root, int move, int depth, Colour opponent)
    {
        if ((outcomes & MoveOutcome.Promotion) == 0)
        {
            return (false, 0, 0);
        }

        var checks = 0;

        var checkMates = 0;

        for (var kind = Kind.Rook; kind < Kind.King; kind++)
        {
            var copy = new Game(game);
            
            copy.PromotePawn(move, kind);

            if (copy.IsKingInCheck(opponent))
            {
                outcomes |= MoveOutcome.Check;

                checks++;

                if (! CanMove(copy, opponent))
                {
                    outcomes |= MoveOutcome.CheckMate;

                    checkMates++;
                }
            }

            if (depth > 1)
            {
                Enqueue(copy, depth - 1, root, CalculatePriority(copy, outcomes, move, kind, opponent));
            }
        }

        return (true, checks, checkMates);
    }

    private int CalculatePriority(Game game, MoveOutcome outcome, int target, Kind player, Colour opponent)
    {
        var priority = (MoveOutcome.CheckMate - outcome) * 1_000_000;

        if ((outcome & MoveOutcome.Capture) > 0)
        {
            if ((outcome & MoveOutcome.EnPassant) > 0)
            {
                priority += (10 - Scores.Pawn) * 1_000;
            }
            else
            {
                var capturedPiece = game.GetKind(target);
        
                priority += (10 - _pieceCache[capturedPiece].Value) * 1_000;
            }
        
            priority += _pieceCache[player].Value * 100;
        }

        // priority += game.CellHasAttackers(target, opponent) ? 10_000 : 0;
        
        return priority;
    }

    private bool CanMove(Game game, Colour colour)
    {
        var pieces = game[colour];

        var cell = pieces.PopBit();

        while (cell > -1)
        {
            var kind = game.GetKind(cell);

            var moves = _pieceCache[kind].GetMoves(game, cell);

            var move = moves.PopBit();

            while (move > -1)
            {
                var copy = new Game(game);

                copy.MakeMove(cell, move);

                if (copy.IsKingInCheck(colour))
                {
                    move = moves.PopBit();
                
                    continue;
                }

                return true;
            }

            cell = pieces.PopBit();
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

    private void Enqueue(Game game, int depth, int root, int priority)
    {
        // ReSharper disable once InconsistentlySynchronizedField - Doesn't need to be exactly 1,000.
        if (_centralQueue.Count < CentralPoolMax)
        {
            lock (_centralQueue)
            {
                _centralQueue.Enqueue((game, depth, root), priority);
            }
        }
        else
        {
            _localQueue.Enqueue((game, depth, root), priority);
        }
    }
}