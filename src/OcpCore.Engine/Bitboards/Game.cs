using System.Numerics;
using System.Runtime.CompilerServices;
using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Bitboards;

public class Game
{
    private readonly ulong[] _planes;

    private readonly Moves _moves = Moves.Instance;
    
    public State State { get; private set; } 

    public ulong this[Plane plane]
    {
        get => _planes[(int) plane];
        private set => _planes[(int) plane] = value;
    }
    
    public Game()
    {
        _planes = new ulong[Enum.GetValues<Plane>().Length];

        State = new State();
    }

    public Game(Game game)
    {
        var planeCount = Enum.GetValues<Plane>().Length;
        
        _planes = new ulong[planeCount];
        
        Buffer.BlockCopy(game._planes, 0, _planes, 0, planeCount * sizeof(ulong));

        State = new State(game.State);
    }

    public bool IsKind(Kind kind, int cell)
    {
        return (this[(Plane) kind] & (1ul << cell)) > 0;
    }

    public bool IsEmpty(int cell)
    {
        return ((this[Plane.White] | this[Plane.Black]) & (1ul << cell)) == 0;
    }

    public Kind GetKind(int cell)
    {
        return (Kind) GetKindInternal(1ul << cell);
    }
    
    public MoveOutcome MakeMove(int from, int to)
    {
        var fromBit = 1ul << from;

        var toBit = 1ul << to;
        
        if (((this[Plane.White] | this[Plane.Black]) & fromBit) == 0)
        {
            throw new InvalidMoveException($"No piece at {from.ToStandardNotation()}.");
        }

        var player = (this[Plane.White] & fromBit) == fromBit ? Plane.White : Plane.Black;

        if ((Colour) player != State.Player)
        {
            throw new InvalidMoveException($"Not the turn for {player}.");
        }

        var kind = GetKindInternal(fromBit);

        var outcome = MoveOutcome.Move;

        if (kind == Plane.King)
        {
            HandleKingSpecifics(from, to, ref outcome);
        }

        if (IsColour(player.InvertColour(), to))
        {
            HandleCapture(to, ref outcome);
        }

        if (kind == Plane.Pawn)
        {
            HandlePawnSpecifics(to, ref outcome);
        }
        
        UpdateBitboards(kind, player, fromBit, toBit);

        UpdateEnPassantState(kind, from, to);
        
        UpdateCastleState(kind, player, from);
        
        State.InvertPlayer();

        return outcome;
    }

    public bool IsKingInCheck(Plane colour, int probePosition = -1)
    {
        var position = colour == Plane.White ? State.WhiteKingCell : State.BlackKingCell;

        if (probePosition > -1)
        {
            position = probePosition;
        }

        var opponentPlane = colour.InvertColour();
    
        var attacks = _moves[MoveSet.Knight][position];
        
        if ((attacks & this[opponentPlane] & this[Plane.Knight]) > 0)
        {
            return true;
        }

        attacks = Piece.GetDiagonalSlidingMoves(this, colour, opponentPlane, position)
                  | Piece.GetAntiDiagonalSlidingMoves(this, colour, opponentPlane, position);
        
        if ((attacks & (this[Plane.Bishop] | this[Plane.Queen])) > 0)
        {
            return true;
        }

        attacks = Piece.GetHorizontalSlidingMoves(this, colour, opponentPlane, position)
                  | Piece.GetVerticalSlidingMoves(this, colour, opponentPlane, position);
        
        if ((attacks & (this[Plane.Rook] | this[Plane.Queen])) > 0)
        {
            return true;
        }

        attacks = _moves[colour == Plane.White ? MoveSet.PawnWhiteAttack : MoveSet.PawnBlackAttack][position];
        
        if ((this[Plane.Pawn] & this[opponentPlane] & attacks) > 0)
        {
            return true;
        }

        attacks = _moves[MoveSet.King][position];

        if ((attacks & this[opponentPlane] & this[Plane.King]) > 0)
        {
            return true;
        }
        
        return false;
    }

    public void ParseFen(string fen)
    {
        State = FenInterface.ParseFen(fen, _planes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleCapture(int to, ref MoveOutcome outcome)
    {
        outcome |= MoveOutcome.Capture;

        if ((this[Plane.Rook] & (1ul << to)) > 0)
        {
            var file = Cell.GetFile(to);

            if (State.Player == Colour.White)
            {
                State.RemoveCastleRights(file == 0 ? Castle.BlackQueenSide : Castle.BlackKingSide);
            }
            else
            {
                State.RemoveCastleRights(file == 0 ? Castle.WhiteQueenSide : Castle.WhiteKingSide);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void HandlePawnSpecifics(int to, ref MoveOutcome outcome)
    {
        if (to == State.EnPassantTarget)
        {
            outcome |= MoveOutcome.EnPassant | MoveOutcome.Capture;

            var target = 1ul << (State.EnPassantTarget.Value + (State.Player == (Colour) Plane.White ? -Constants.Files : Constants.Files));

            var clearMask = ~target;
                
            this[Plane.White] &= clearMask;

            this[Plane.Black] &= clearMask;

            this[Plane.Pawn] &= clearMask;
        }

        if (Cell.GetRank(to) is 0 or 7)
        {
            outcome |= MoveOutcome.Promotion;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleKingSpecifics(int from, int to, ref MoveOutcome outcome)
    {
        if (State.Player == Colour.White)
        {
            State.SetWhiteKingCell(to);
        }
        else
        {
            State.SetBlackKingCell(to);
        }

        if (Math.Abs(from - to) == 2)
        {
            outcome |= MoveOutcome.Castle;
            
            if (from < to)
            {
                UpdateBitboards(Plane.Rook, (Plane) State.Player, 1ul << from << 3, 1ul << from << 1);
            }
            else
            {
                UpdateBitboards(Plane.Rook, (Plane) State.Player, 1ul << from >> 4, 1ul << from >> 1);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsColour(Plane colour, int cell)
    {
        return (this[colour] & (1ul << cell)) > 0;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateEnPassantState(Plane kind, int position, int target)
    {
        if (kind == Plane.Pawn)
        {
            var delta = position - target;

            if (Math.Abs(delta) == Constants.Ranks * 2)
            {
                State.SetEnPassantTarget(delta > 0 ? position - Constants.Files : position + Constants.Files);
                
                return;
            }
        }

        State.SetEnPassantTarget(null);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateCastleState(Plane kind, Plane colour, int position)
    {
        if (kind is not (Plane.Rook or Plane.King))
        {
            return;
        }

        if (kind == Plane.King)
        {
            State.RemoveCastleRights(colour == Plane.White ? Castle.White : Castle.Black);
            
            return;
        }

        if (Cell.GetFile(position) == 0)
        {
            State.RemoveCastleRights(colour == Plane.White ? Castle.WhiteQueenSide : Castle.BlackQueenSide);
            
            return;
        }

        State.RemoveCastleRights(colour == Plane.White ? Castle.WhiteKingSide : Castle.BlackKingSide);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Plane GetKindInternal(ulong cellBit)
    {
        if ((this[Plane.Pawn] & cellBit) == cellBit)
        {
            return Plane.Pawn;
        }
        
        if ((this[Plane.Rook] & cellBit) == cellBit)
        {
            return Plane.Rook;
        }
        
        if ((this[Plane.Knight] & cellBit) == cellBit)
        {
            return Plane.Knight;
        }
        
        if ((this[Plane.Bishop] & cellBit) == cellBit)
        {
            return Plane.Bishop;
        }
        
        if ((this[Plane.Queen] & cellBit) == cellBit)
        {
            return Plane.Queen;
        }
        
        if ((this[Plane.King] & cellBit) == cellBit)
        {
            return Plane.King;
        }

        throw new InvalidMoveException($"No piece at {BitOperations.TrailingZeroCount(cellBit).ToStandardNotation()}.");
    }

    private void UpdateBitboards(Plane kind, Plane colour, ulong fromBit, ulong toBit)
    {
        var clearMask = ~fromBit & ~toBit;

        this[Plane.White] &= clearMask;
        this[Plane.Black] &= clearMask;

        this[Plane.Pawn] &= clearMask;
        this[Plane.Rook] &= clearMask;
        this[Plane.Knight] &= clearMask;
        this[Plane.Bishop] &= clearMask;
        this[Plane.Queen] &= clearMask;
        this[Plane.King] &= clearMask;

        this[colour] |= toBit;

        this[kind] |= toBit;
    }
}