using System.Numerics;
using System.Runtime.CompilerServices;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Kernel;

public class StateProcessor
{
    private const int CentralPoolMax = 1_000;
    
    private readonly PriorityQueue<Node, int> _centralQueue;
    
    private readonly PriorityQueue<Node, int> _localQueue = new();
    
    private readonly PieceCache _pieceCache = PieceCache.Instance;

    private readonly PerfTestCollector _perfTestCollector;

    private readonly Colour _engineColour;

    private readonly Dictionary<int, (int Score, PlyOutcome Outcome, string Move)> _bestMoves = [];

    private int _maxDepth;

    private long[] _depthCounts;

    private long[][] _outcomes;

    private Action<StateProcessor, bool> _callback;

    public IReadOnlyDictionary<int, (int Score, PlyOutcome Outcome, string Move)> BestMoves => _bestMoves;

    public long GetDepthCount(int ply) => _depthCounts[ply];

    public long GetOutcomeCount(int ply, PlyOutcome outcome) => _outcomes[ply][BitOperations.Log2((byte) outcome) + 1];

    public StateProcessor(Colour engineColour, PriorityQueue<Node, int> centralQueue, PerfTestCollector perfTestCollector = null)
    {
        _engineColour = engineColour;
        
        _centralQueue = centralQueue;

        _perfTestCollector = perfTestCollector;
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

                var node = _localQueue.Dequeue();
                
                ProcessWorkItem(node);
            }
        }

        callback(this, true);
    }

    private void ProcessWorkItem(Node node)
    {
        var game = node.Game;
        
        var pieces = game[game.State.Player];

        var from = pieces.PopRandomBit();

        while (from > -1)
        {
            var kind = game.GetKind(from);

            var moves = _pieceCache[kind].GetMoves(game, from);

            var to = moves.PopRandomBit();

            while (to > -1)
            {
                ProcessMove(node, kind, from, to);

                to = moves.PopRandomBit();
            }

            from = pieces.PopRandomBit();
        }
    }

    private void ProcessMove(Node node, Kind kind, int from, int to)
    {
        var (game, depth, root) = (node.Game, node.Depth, node.Root);

        var copy = new Game(game);

        var outcomes = copy.MakeMove(from, to);

        var player = game.State.Player;

        if (copy.IsKingInCheck(player))
        {
            return;
        }

        var opponent = player.Invert();

        var ply = _maxDepth - depth + 1;

        if (HandlePromotion(ref outcomes, copy, ply, root, from, to, depth, opponent))
        {
            return;
        }

        IncrementCounts(ply, 1, ref root, from, to);

        if (copy.IsKingInCheck(opponent))
        {
            outcomes |= PlyOutcome.Check;

            if (! CanMove(copy, opponent))
            {
                outcomes |= PlyOutcome.CheckMate;
            }
        }

        IncrementOutcomes(ply, outcomes);

        if (depth > 1 && (outcomes & (PlyOutcome.CheckMate | PlyOutcome.Promotion)) == 0)
        {
            Enqueue(copy, depth - 1, root, CalculatePriority(game, outcomes, to, kind, opponent));
        }
        
        var score = _engineColour == Colour.Black ? game.State.BlackScore : game.State.WhiteScore;

        bool addToBestScores;

        if (! _bestMoves.TryGetValue(node.Depth, out var bestMove))
        {
            addToBestScores = true;
        }
        else
        {
            addToBestScores = score > bestMove.Score;
        }

        if (addToBestScores)
        {
            _bestMoves[node.Depth] = (score, outcomes, $"{from.ToStandardNotation()}{to.ToStandardNotation()}");
        }
    }

    private bool HandlePromotion(ref PlyOutcome outcomes, Game game, int ply, int root, int from, int to, int depth, Colour opponent)
    {
        if ((outcomes & PlyOutcome.Promotion) == 0)
        {
            return false;
        }

        var checks = 0;

        var checkmates = 0;

        for (var kind = Kind.Rook; kind < Kind.King; kind++)
        {
            var copy = new Game(game);
            
            copy.PromotePawn(to, kind);

            if (copy.IsKingInCheck(opponent))
            {
                outcomes |= PlyOutcome.Check;

                checks++;

                if (! CanMove(copy, opponent))
                {
                    outcomes |= PlyOutcome.CheckMate;

                    checkmates++;
                }
            }

            if (depth > 1)
            {
                Enqueue(copy, depth - 1, root, CalculatePriority(copy, outcomes, to, kind, opponent));
            }
        }
        
        IncrementCounts(ply, 4, ref root, from, to);

        IncrementPromotionOutcomes(ply, outcomes, checks, checkmates);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculatePriority(Game game, PlyOutcome outcome, int target, Kind player, Colour opponent)
    {
        var priority = ((int) PlyOutcome.CheckMate - (1 << BitOperations.Log2((uint) outcome))) * 100;

        if ((outcome & PlyOutcome.Capture) > 0)
        {
            if ((outcome & PlyOutcome.EnPassant) > 0)
            {
                priority += (10 - Scores.Pawn) * 10;
            }
            else
            {
                var capturedPiece = game.GetKind(target);
        
                priority += (10 - _pieceCache[capturedPiece].Value) * 10;
            }
        
            priority += _pieceCache[player].Value;
        }

        priority += game.CellHasAttackers(target, opponent) ? 10_000 : 0;
        
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IncrementCounts(int ply, int count, ref int root, int from, int to)
    {
        _depthCounts[ply] += count;

        if (_depthCounts[ply] > 1_000)
        {
            _callback(this, false);

            for (var i = 0; i <= _maxDepth; i++)
            {
                _depthCounts[i] = 0;
            }
        }
        
        if (_perfTestCollector != null)
        {
            if (ply == 1)
            {
                root = from << 8 | to;
            }

            _perfTestCollector.AddCount(ply, _maxDepth, root, count);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IncrementOutcomes(int ply, PlyOutcome outcomes)
    {
        while (outcomes > 0)
        {
            var outcome = BitOperations.TrailingZeroCount((int) outcomes);

            _outcomes[ply][outcome + 1]++;

            outcomes ^= (PlyOutcome) (1 << outcome);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IncrementPromotionOutcomes(int ply, PlyOutcome outcomes, int checks, int checkmates)
    {
        _outcomes[ply][BitOperations.TrailingZeroCount((int) PlyOutcome.Move) + 1] += 4;
                
        if ((outcomes & PlyOutcome.Capture) > 0)
        {
            _outcomes[ply][BitOperations.TrailingZeroCount((int) PlyOutcome.Capture) + 1] += 4;
        }

        _outcomes[ply][BitOperations.TrailingZeroCount((int) PlyOutcome.Promotion) + 1] += 4;

        _outcomes[ply][BitOperations.TrailingZeroCount((int) PlyOutcome.Check) + 1] += checks;

        _outcomes[ply][BitOperations.TrailingZeroCount((int) PlyOutcome.CheckMate) + 1] += checkmates;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Enqueue(Game game, int depth, int root, int priority)
    {
        // ReSharper disable once InconsistentlySynchronizedField - Doesn't need to be exactly 1,000.
        if (_centralQueue.Count < CentralPoolMax)
        {
            lock (_centralQueue)
            {
                _centralQueue.Enqueue(new Node(game, depth, root), priority);
            }
        }
        else
        {
            _localQueue.Enqueue(new Node(game, depth, root), priority);
        }
    }
}