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

    public ulong this[Colour colour]
    {
        get => _planes[(int) colour];
        private set => _planes[(int) colour] = value;
    }

    public ulong this[Kind kind]
    {
        get => _planes[(int) kind];
        private set => _planes[(int) kind] = value;
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
        return (this[kind] & (1ul << cell)) > 0;
    }

    public bool IsEmpty(int cell)
    {
        return ((this[Colour.White] | this[Colour.Black]) & (1ul << cell)) == 0;
    }

    public Kind GetKind(int cell)
    {
        return GetKindInternal(1ul << cell);
    }
    
    public MoveOutcome MakeMove(int from, int to)
    {
        var fromBit = 1ul << from;

        var toBit = 1ul << to;
        
        if (((this[Colour.White] | this[Colour.Black]) & fromBit) == 0)
        {
            throw new InvalidMoveException($"No piece at {from.ToStandardNotation()}.");
        }

        var player = (this[Colour.White] & fromBit) == fromBit ? Colour.White : Colour.Black;

        if (player != State.Player)
        {
            throw new InvalidMoveException($"Not the turn for {player}.");
        }

        var kind = GetKindInternal(fromBit);

        var outcome = MoveOutcome.Move;

        if (kind == Kind.King)
        {
            HandleKingSpecifics(from, to, ref outcome);
        }

        if (IsColour(player.Invert(), to))
        {
            HandleCapture(to, ref outcome);
        }

        if (kind == Kind.Pawn)
        {
            HandlePawnSpecifics(to, ref outcome);
        }
        
        UpdateBitboards(kind, player, fromBit, toBit);

        UpdateEnPassantState(kind, from, to);
        
        UpdateCastleState(kind, player, from);
        
        State.InvertPlayer();

        return outcome;
    }

    public bool IsKingInCheck(Colour colour, int probePosition = -1)
    {
        var position = colour == Colour.White ? State.WhiteKingCell : State.BlackKingCell;

        if (probePosition > -1)
        {
            position = probePosition;
        }

        var opponentColour = colour.Invert();
    
        var attacks = _moves[MoveSet.Knight][position];
        
        if ((attacks & this[opponentColour] & this[Kind.Knight]) > 0)
        {
            return true;
        }

        attacks = Piece.GetDiagonalSlidingMoves(this, colour, opponentColour, position)
                  | Piece.GetAntiDiagonalSlidingMoves(this, colour, opponentColour, position);
        
        if ((attacks & (this[Kind.Bishop] | this[Kind.Queen])) > 0)
        {
            return true;
        }

        attacks = Piece.GetHorizontalSlidingMoves(this, colour, opponentColour, position)
                  | Piece.GetVerticalSlidingMoves(this, colour, opponentColour, position);
        
        if ((attacks & (this[Kind.Rook] | this[Kind.Queen])) > 0)
        {
            return true;
        }

        attacks = _moves[colour == Colour.White ? MoveSet.PawnWhiteAttack : MoveSet.PawnBlackAttack][position];
        
        if ((this[Kind.Pawn] & this[opponentColour] & attacks) > 0)
        {
            return true;
        }

        attacks = _moves[MoveSet.King][position];

        if ((attacks & this[opponentColour] & this[Kind.King]) > 0)
        {
            return true;
        }
        
        return false;
    }

    public int CountCellAttackers(int cell, Colour opponentColour)
    {
        var mask = _moves[MoveSet.Knight][cell] & this[Kind.Knight];

        mask |= _moves[MoveSet.King][cell] & this[Kind.King];

        var player = opponentColour.Invert();

        mask |= _moves[player == Colour.White ? MoveSet.PawnWhiteAttack : MoveSet.PawnBlackAttack][cell] & this[Kind.Pawn];

        mask &= this[opponentColour];

        var count = BitOperations.PopCount(mask);

        var attacks = Piece.GetDiagonalSlidingMoves(this, player, opponentColour, cell)
                      | Piece.GetAntiDiagonalSlidingMoves(this, player, opponentColour, cell);

        count += BitOperations.PopCount(attacks & this[Kind.Bishop]);

        count += BitOperations.PopCount(attacks & this[Kind.Queen]);

        attacks = Piece.GetHorizontalSlidingMoves(this, player, opponentColour, cell)
                  | Piece.GetVerticalSlidingMoves(this, player, opponentColour, cell);

        count += BitOperations.PopCount(attacks & this[Kind.Rook]);

        count += BitOperations.PopCount(attacks & this[Kind.Queen]);

        return count;
    }

    public void PromotePawn(int cell, Kind kind)
    {
        if (! IsKind(Kind.Pawn, cell))
        {
            throw new InvalidMoveException($"There isn't a Pawn at {cell.ToStandardNotation()}.");
        }

        var cellBit = 1ul << cell;

        this[Kind.Pawn] &= ~cellBit;

        this[kind] |= cellBit;
    }

    public void ParseFen(string fen)
    {
        State = FenInterface.ParseFen(fen, _planes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleCapture(int to, ref MoveOutcome outcome)
    {
        outcome |= MoveOutcome.Capture;

        if ((this[Kind.Rook] & (1ul << to)) > 0)
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

            var target = 1ul << (State.EnPassantTarget.Value + (State.Player == Colour.White ? -Constants.Files : Constants.Files));

            var clearMask = ~target;
                
            this[Colour.White] &= clearMask;

            this[Colour.Black] &= clearMask;

            this[Kind.Pawn] &= clearMask;
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
                UpdateBitboards(Kind.Rook, State.Player, 1ul << from << 3, 1ul << from << 1);
            }
            else
            {
                UpdateBitboards(Kind.Rook, State.Player, 1ul << from >> 4, 1ul << from >> 1);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsColour(Colour colour, int cell)
    {
        return (this[colour] & (1ul << cell)) > 0;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateEnPassantState(Kind kind, int position, int target)
    {
        if (kind == Kind.Pawn)
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
    private void UpdateCastleState(Kind kind, Colour colour, int position)
    {
        if (kind is not (Kind.Rook or Kind.King))
        {
            return;
        }

        if (kind == Kind.King)
        {
            State.RemoveCastleRights(colour == Colour.White ? Castle.White : Castle.Black);
            
            return;
        }

        if (Cell.GetFile(position) == 0)
        {
            State.RemoveCastleRights(colour == Colour.White ? Castle.WhiteQueenSide : Castle.BlackQueenSide);
            
            return;
        }

        State.RemoveCastleRights(colour == Colour.White ? Castle.WhiteKingSide : Castle.BlackKingSide);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Kind GetKindInternal(ulong cellBit)
    {
        if ((this[Kind.Pawn] & cellBit) == cellBit)
        {
            return Kind.Pawn;
        }
        
        if ((this[Kind.Rook] & cellBit) == cellBit)
        {
            return Kind.Rook;
        }
        
        if ((this[Kind.Knight] & cellBit) == cellBit)
        {
            return Kind.Knight;
        }
        
        if ((this[Kind.Bishop] & cellBit) == cellBit)
        {
            return Kind.Bishop;
        }
        
        if ((this[Kind.Queen] & cellBit) == cellBit)
        {
            return Kind.Queen;
        }
        
        if ((this[Kind.King] & cellBit) == cellBit)
        {
            return Kind.King;
        }

        throw new InvalidMoveException($"No piece at {BitOperations.TrailingZeroCount(cellBit).ToStandardNotation()}.");
    }

    private void UpdateBitboards(Kind kind, Colour colour, ulong fromBit, ulong toBit)
    {
        var clearMask = ~fromBit & ~toBit;

        this[Colour.White] &= clearMask;
        this[Colour.Black] &= clearMask;

        this[Kind.Pawn] &= clearMask;
        this[Kind.Rook] &= clearMask;
        this[Kind.Knight] &= clearMask;
        this[Kind.Bishop] &= clearMask;
        this[Kind.Queen] &= clearMask;
        this[Kind.King] &= clearMask;

        this[colour] |= toBit;

        this[kind] |= toBit;
    }
}