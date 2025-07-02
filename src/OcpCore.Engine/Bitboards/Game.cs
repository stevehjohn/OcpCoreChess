using System.Numerics;
using System.Runtime.CompilerServices;
using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Bitboards;

public struct Game
{
    private Planes _planes;

    private readonly MoveCache _moves = MoveCache.Instance;
    
    private readonly PieceCache _pieceCache = PieceCache.Instance;

    public State State { get; private set; } 

    public ulong this[Colour colour]
    {
        get
        {
            return colour switch
            {
                Colour.White => _planes[Planes.White],
                _ => _planes[Planes.Black]
            };
        }
        private set
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (colour)
            {
                case Colour.White:
                    _planes[Planes.White] = value;
                    break;
                case Colour.Black:
                    _planes[Planes.Black] = value;
                    break;
            }
        }
    }

    public ulong this[Kind kind]
    {
        get
        {
            return kind switch
            {
                Kind.Pawn => _planes[Planes.Pawn],
                Kind.Rook => _planes[Planes.Rook],
                Kind.Knight => _planes[Planes.Knight],
                Kind.Bishop => _planes[Planes.Bishop],
                Kind.Queen => _planes[Planes.Queen],
                _ => _planes[Planes.King]
            };
        }

        private set
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (kind)
            {
                case Kind.Pawn:
                    _planes[Planes.Pawn] = value;
                    break;
                case Kind.Rook:
                    _planes[Planes.Rook] = value;
                    break;
                case Kind.Knight:
                    _planes[Planes.Knight] = value;
                    break;
                case Kind.Bishop:
                    _planes[Planes.Bishop] = value;
                    break;
                case Kind.Queen:
                    _planes[Planes.Queen] = value;
                    break;
                case Kind.King:
                    _planes[Planes.King] = value;
                    break;
            }
        }
    }

    public Game()
    {
        State = new State();
    }

    public Game(Game game)
    {
        _planes = game._planes;

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
    
    public PlyOutcome MakeMove(int from, int to)
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

        var outcome = PlyOutcome.Move;

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

        var opponentMask = this[opponentColour];

        var moves = _moves[position];
        
        var attacks = moves[MoveSets.Knight];
        
        if ((attacks & opponentMask & this[Kind.Knight]) > 0)
        {
            return true;
        }
        
        attacks = moves[colour == Colour.White ? MoveSets.PawnWhiteAttack : MoveSets.PawnBlackAttack];
        
        if ((this[Kind.Pawn] & opponentMask & attacks) > 0)
        {
            return true;
        }

        attacks = moves[MoveSets.King];

        if ((attacks & opponentMask & this[Kind.King]) > 0)
        {
            return true;
        }
        
        var pieces = this[Kind.Bishop] | this[Kind.Queen];

        if ((pieces & opponentMask & moves[MoveSets.Diagonal]) > 0)
        {
            attacks = Piece.GetDiagonalSlidingMoves(this, colour, opponentColour, position);

            if ((attacks & pieces) > 0)
            {
                return true;
            }
        }

        if ((pieces & opponentMask & moves[MoveSets.AntiDiagonal]) > 0)
        {
            attacks = Piece.GetAntiDiagonalSlidingMoves(this, colour, opponentColour, position);

            if ((attacks & pieces) > 0)
            {
                return true;
            }
        }

        pieces = this[Kind.Rook] | this[Kind.Queen];

        if ((pieces & opponentMask & moves[MoveSets.Horizontal]) > 0)
        {
            attacks = Piece.GetHorizontalSlidingMoves(this, colour, opponentColour, position);

            if ((attacks & pieces) > 0)
            {
                return true;
            }
        }

        if ((pieces & opponentMask & moves[MoveSets.Vertical]) > 0)
        {
            attacks = Piece.GetVerticalSlidingMoves(this, colour, opponentColour, position);

            if ((attacks & pieces) > 0)
            {
                return true;
            }
        }

        return false;
    }

    public bool CellHasAttackers(int cell, Colour opponentColour)
    {
        var moves = _moves[cell];
        
        var mask = moves[MoveSets.Knight] & this[Kind.Knight];

        if (mask > 0)
        {
            return true;
        }

        mask = moves[MoveSets.King] & this[Kind.King];

        if (mask > 0)
        {
            return true;
        }

        var player = opponentColour.Invert();

        mask = moves[player == Colour.White ? MoveSets.PawnWhiteAttack : MoveSets.PawnBlackAttack] & this[Kind.Pawn];

        mask &= this[opponentColour];

        if (mask > 0)
        {
            return true;
        }

        var attacks = Piece.GetDiagonalSlidingMoves(this, player, opponentColour, cell)
                      | Piece.GetAntiDiagonalSlidingMoves(this, player, opponentColour, cell);

        if ((attacks & this[Kind.Bishop]) > 0)
        {
            return true;
        }

        if ((attacks & this[Kind.Queen]) > 0)
        {
            return true;
        }

        attacks = Piece.GetHorizontalSlidingMoves(this, player, opponentColour, cell)
                  | Piece.GetVerticalSlidingMoves(this, player, opponentColour, cell);

        if ((attacks & this[Kind.Rook]) > 0)
        {
            return true;
        }

        if ((attacks & this[Kind.Queen]) > 0)
        {
            return true;
        }

        return false;
    }

    public void PromotePawn(int cell, Kind kind)
    {
        var cellBit = 1ul << cell;

        this[Kind.Pawn] &= ~cellBit;

        this[kind] |= cellBit;
    }

    public void ParseFen(string fen)
    {
        State = FenInterface.ParseFen(fen, ref _planes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleCapture(int to, ref PlyOutcome outcome)
    {
        outcome |= PlyOutcome.Capture;

        var capturedKind = GetKind(to);

        if (State.Player == Colour.White)
        {
            State.UpdateBlackScore(-_pieceCache[capturedKind].Value);
        }
        else
        {
            State.UpdateWhiteScore(-_pieceCache[capturedKind].Value);
        }

        if ((this[Kind.Rook] & (1ul << to)) > 0)
        {
            var file = Cell.GetFile(to);

            if (file is not (0 or 7))
            {
                return;
            }

            var rank = Cell.GetRank(to);

            if (rank is not (0 or 7))
            {
                return;
            }

            if (State.Player == Colour.White && rank == 7)
            {
                State.RemoveCastleRights(file == 0 ? Castle.BlackQueenSide : Castle.BlackKingSide);
            }
            else if (rank == 0)
            {
                State.RemoveCastleRights(file == 0 ? Castle.WhiteQueenSide : Castle.WhiteKingSide);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void HandlePawnSpecifics(int to, ref PlyOutcome outcome)
    {
        if (to == State.EnPassantTarget)
        {
            outcome |= PlyOutcome.EnPassant | PlyOutcome.Capture;

            var target = 1ul << (State.EnPassantTarget.Value + (State.Player == Colour.White ? -Constants.Files : Constants.Files));

            var clearMask = ~target;
                
            this[Colour.White] &= clearMask;

            this[Colour.Black] &= clearMask;

            this[Kind.Pawn] &= clearMask;
            
            if (State.Player == Colour.White)
            {
                State.UpdateBlackScore(-Scores.Pawn);
            }
            else
            {
                State.UpdateWhiteScore(-Scores.Pawn);
            }
        }

        if (Cell.GetRank(to) is 0 or 7)
        {
            outcome |= PlyOutcome.Promotion;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleKingSpecifics(int from, int to, ref PlyOutcome outcome)
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
            outcome |= PlyOutcome.Castle;
            
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

        var file = Cell.GetFile(position);

        switch (file)
        {
            case 0:
                State.RemoveCastleRights(colour == Colour.White ? Castle.WhiteQueenSide : Castle.BlackQueenSide);
            
                return;
            case 7:
                State.RemoveCastleRights(colour == Colour.White ? Castle.WhiteKingSide : Castle.BlackKingSide);
                break;
        }
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