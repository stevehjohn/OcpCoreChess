using System.Numerics;
using System.Runtime.CompilerServices;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Kernel;

public class StateProcessor
{
    private readonly Queue<Node> _centralQueue;

    private readonly PieceCache _pieceCache = PieceCache.Instance;

    private readonly PerfTestCollector _perfTestCollector;

    private int _maxDepth;

    private long[] _depthCounts;

    private long[][] _outcomes;

    private CancellationToken _cancellationToken;

    private List<Node> _seeds;

    public long GetDepthCount(int ply)
    {
        var counts = Volatile.Read(ref _depthCounts);

        return counts == null ? 0 : Volatile.Read(ref counts[ply]);
    }

    public long GetOutcomeCount(int ply, PlyOutcome outcome)
    {
        var outcomes = Volatile.Read(ref _outcomes);

        return outcomes == null ? 0 : Volatile.Read(ref outcomes[ply][BitOperations.Log2((byte) outcome) + 1]);
    }

    public StateProcessor(Colour engineColour, Queue<Node> centralQueue, PerfTestCollector perfTestCollector = null)
    {
        _centralQueue = centralQueue;

        _perfTestCollector = perfTestCollector;
    }

    private void Initialise(int maxDepth, CancellationToken cancellationToken)
    {
        _maxDepth = maxDepth;

        _cancellationToken = cancellationToken;

        Volatile.Write(ref _depthCounts, new long[maxDepth + 1]);

        var outcomes = new long[maxDepth + 1][];

        for (var ply = 0; ply <= maxDepth; ply++)
        {
            outcomes[ply] = new long[Constants.MoveOutcomes + 1];
        }

        Volatile.Write(ref _outcomes, outcomes);
    }

    internal List<Node> PrepareWork(Game game, int maxDepth, CancellationToken cancellationToken)
    {
        Initialise(maxDepth, cancellationToken);

        _seeds = [];

        ProcessWorkItem(new Node(game, maxDepth, -1));

        var seeds = _seeds;

        _seeds = null;

        return seeds;
    }

    public void StartProcessing(int maxDepth, Action<StateProcessor, bool> callback, CancellationToken cancellationToken)
    {
        Initialise(maxDepth, cancellationToken);

        while (true)
        {
            Node node;

            lock (_centralQueue)
            {
                if (_centralQueue.Count == 0)
                {
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                node = _centralQueue.Dequeue();
            }

            ProcessWorkItem(node);
        }

        callback?.Invoke(this, true);
    }

    private void ProcessWorkItem(Node node)
    {
        if (_cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var game = node.Game;
        
        var pieces = game[game.State.Player];

        var from = pieces.PopBit();

        while (from > -1)
        {
            var kind = game.GetKind(from);

            var moves = _pieceCache[kind].GetMoves(game, from);

            var to = moves.PopBit();

            while (to > -1)
            {
                ProcessMove(node, from, to);

                to = moves.PopBit();
            }

            from = pieces.PopBit();
        }
    }

    private void ProcessMove(Node node, int from, int to)
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

        if (HandlePromotion(outcomes, copy, ply, root, from, to, depth, opponent))
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
            Continue(copy, depth - 1, root);
        }
    }

    private bool HandlePromotion(PlyOutcome outcomes, Game game, int ply, int root, int from, int to, int depth, Colour opponent)
    {
        if ((outcomes & PlyOutcome.Promotion) == 0)
        {
            return false;
        }

        if (ply == 1)
        {
            root = from << 8 | to;
        }

        for (var kind = Kind.Rook; kind < Kind.King; kind++)
        {
            var copy = new Game(game);

            copy.PromotePawn(to, kind);

            var promotionOutcomes = outcomes;

            if (copy.IsKingInCheck(opponent))
            {
                promotionOutcomes |= PlyOutcome.Check;

                if (! CanMove(copy, opponent))
                {
                    promotionOutcomes |= PlyOutcome.CheckMate;
                }
            }

            IncrementCounts(ply, 1, ref root, from, to);

            IncrementOutcomes(ply, promotionOutcomes);

            if (depth > 1 && (promotionOutcomes & PlyOutcome.CheckMate) == 0)
            {
                Continue(copy, depth - 1, root);
            }
        }

        return true;
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

    private void Continue(Game game, int depth, int root)
    {
        var node = new Node(game, depth, root);

        if (_seeds != null)
        {
            _seeds.Add(node);
        }
        else
        {
            ProcessWorkItem(node);
        }
    }
}
