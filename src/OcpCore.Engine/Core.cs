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

    private Task _getMoveTask;

    public long GetDepthCount(int ply) => _coordinator.GetDepthCount(ply);

    public long GetOutcomeCount(int ply, PlyOutcome outcome) => _coordinator.GetOutcomeCount(ply, outcome);

    public bool IsBusy => _cancellationTokenSource != null;

    public int QueueSize => _coordinator?.QueueSize ?? 0;

    public IReadOnlyDictionary<string, long> PerftData => _perfTestCollector?.Counts;

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    public Colour Player => _engineColour;

    public Colour CurrentPlayer => _game.State.Player;
    
    public (Colour Colour, Kind Kind) this[string position] 
    {
        get
        {
            var cell = position.FromStandardNotation();

            var colour = _game.GetColour(cell);

            var kind = _game.GetKind(cell);

            return (colour, kind);
        }
    }

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

    public (MoveOutcome Outcome, string Move) GetMove(int depth)
    {
        return GetMoveInternal(depth);
    }
    
    public Task GetMove(int depth, Action<(MoveOutcome Outcome, string Move)> callback)
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

    private (MoveOutcome Outcome, string Move) GetMoveInternal(int depth, Action<(MoveOutcome Outcome, string Move)> callback = null)
    {
        _coordinator = new Coordinator(_engineColour, _perfTestCollector);
        
        _coordinator.StartProcessing(_game, depth);

        var bestMove = _coordinator.BestMoves.Count == 0 ? (Score: 0, Outcome: PlyOutcome.Null, Move: string.Empty) : _coordinator.BestMoves.Last().Value;

        var outcome = bestMove.Outcome switch
        {
            PlyOutcome.CheckMate => _engineColour == _game.State.Player ? MoveOutcome.EngineInCheckmate : MoveOutcome.OpponentInCheckmate,
            PlyOutcome.Null => MoveOutcome.Stalemate,
            _ => MoveOutcome.Move
        };
        
        callback?.Invoke((outcome, bestMove.Move));

        return (outcome, bestMove.Move);
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

    public override string ToString()
    {
        return _game.ToString();
    }
}