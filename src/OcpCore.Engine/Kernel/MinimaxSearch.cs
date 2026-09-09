using System.Numerics;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;

namespace OcpCore.Engine.Kernel;

/// <summary>Fixed-depth minimax with alpha-beta pruning and material evaluation.</summary>
public sealed class MinimaxSearch
{
    public const int MateScore = 100_000;

    private readonly PieceCache _pieceCache = PieceCache.Instance;

    public long NodesVisited { get; private set; }

    public long Cutoffs { get; private set; }

    // Scores are from the side-to-move's perspective. Negamax implements minimax
    // by negating both the score and the search window at each turn.
    public (int Score, string Move) FindBestMove(Game game, int depth, bool useAlphaBeta = true)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(depth, 1);

        NodesVisited = 0;

        Cutoffs = 0;

        return Search(game, depth, 0, -MateScore, MateScore, useAlphaBeta);
    }

    private (int Score, string Move) Search(Game game, int depth, int ply, int alpha, int beta, bool prune)
    {
        NodesVisited++;

        var moves = GetMoves(game);

        // Terminal positions must be recognized even at the depth horizon.
        if (moves.Count == 0)
        {
            return (game.IsKingInCheck(game.State.Player) ? -MateScore + ply : 0, string.Empty);
        }

        if (depth == 0)
        {
            return (Evaluate(game), string.Empty);
        }

        var bestScore = int.MinValue;

        var bestMove = string.Empty;

        foreach (var move in moves)
        {
            var score = -Search(move.Game, depth - 1, ply + 1, -beta, -alpha, prune).Score;

            if (score > bestScore)
            {
                bestScore = score;

                bestMove = move.Move;
            }

            if (prune)
            {
                alpha = Math.Max(alpha, score);

                if (alpha >= beta)
                {
                    Cutoffs++;

                    break;
                }
            }
        }

        return (bestScore, bestMove);
    }

    private int Evaluate(Game game)
    {
        var score = 0;

        for (var kind = Kind.Pawn; kind < Kind.King; kind++)
        {
            score += _pieceCache[kind].Value * (BitOperations.PopCount(game[kind] & game[Colour.White])
                - BitOperations.PopCount(game[kind] & game[Colour.Black]));
        }

        return game.State.Player == Colour.White ? score : -score;
    }

    private List<(Game Game, string Move, int Priority)> GetMoves(Game game)
    {
        var result = new List<(Game Game, string Move, int Priority)>();

        var pieces = game[game.State.Player];

        for (var from = pieces.PopBit(); from >= 0; from = pieces.PopBit())
        {
            var targets = _pieceCache[game.GetKind(from)].GetMoves(game, from);

            for (var to = targets.PopBit(); to >= 0; to = targets.PopBit())
            {
                var copy = new Game(game);

                var outcome = copy.MakeMove(from, to);

                if (copy.IsKingInCheck(game.State.Player))
                {
                    continue;
                }

                var notation = $"{from.ToStandardNotation()}{to.ToStandardNotation()}";

                if ((outcome & PlyOutcome.Promotion) != 0)
                {
                    for (var kind = Kind.Rook; kind < Kind.King; kind++)
                    {
                        var promoted = new Game(copy);

                        promoted.PromotePawn(to, kind);

                        var suffix = kind switch
                        {
                            Kind.Rook => "r",
                            Kind.Knight => "n",
                            Kind.Bishop => "b",
                            _ => "q"
                        };

                        result.Add((promoted, notation + suffix, -Evaluate(promoted)));
                    }
                }
                else
                {
                    result.Add((copy, notation, -Evaluate(copy)));
                }
            }
        }

        // Stable ordering also makes equal-score move selection deterministic.
        return result.OrderByDescending(move => move.Priority).ToList();
    }
}
