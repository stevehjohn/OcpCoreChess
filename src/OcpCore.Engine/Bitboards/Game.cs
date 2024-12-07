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

    private static readonly Moves Moves = new();
    
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

    public MoveOutcome MakeMove(int from, int to)
    {
        var fromBit = 1ul << from;

        var toBit = 1ul << to;
        
        if (((this[Plane.White] | this[Plane.Black]) & fromBit) == 0)
        {
            throw new InvalidMoveException($"No piece at {from.ToStandardNotation()}.");
        }

        var player = (this[Plane.White] & fromBit) == fromBit ? Plane.White : Plane.Black;

        var kind = GetKindInternal(fromBit);

        var outcome = MoveOutcome.Move;

        if (kind == Plane.King)
        {
            if (player == Plane.White)
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

                // TODO: Test
                if (from - to < 0)
                {
                    UpdateBitboards(Plane.Rook, player, fromBit << 4, fromBit << 2);
                }
                else
                {
                    UpdateBitboards(Plane.Rook, player, fromBit >> 3, fromBit >> 1);
                }
            }
        }

        if (IsColour(player.InvertColour(), to))
        {
            outcome |= MoveOutcome.Capture;

            if ((this[Plane.Rook] & toBit) > 0)
            {
                var file = Cell.GetFile(to);

                if (player == Plane.White)
                {
                    State.RemoveCastleRights(file == 0 ? Castle.BlackQueenSide : Castle.BlackKingSide);
                }
                else
                {
                    State.RemoveCastleRights(file == 0 ? Castle.WhiteQueenSide : Castle.WhiteKingSide);
                }
            }
        }

        if (kind == Plane.Pawn)
        {
            if (to == State.EnPassantTarget)
            {
                outcome |= MoveOutcome.EnPassant | MoveOutcome.Capture;

                var target = 1ul << (State.EnPassantTarget.Value + (player == Plane.White ? -Constants.Files : Constants.Files));

                var clearMask = ~target;
                
                this[Plane.White] &= clearMask;

                this[Plane.Black] &= clearMask;

                this[Plane.Pawn] &= clearMask;
            }

            if (Cell.GetRank(to) is 0 or 7)
            {
                outcome |= MoveOutcome.Promotion;

                // TODO: Knight sometimes?
                kind = Plane.Queen;
            }
        }
        
        UpdateBitboards(kind, player, fromBit, toBit);

        UpdateEnPassantState(kind, from, to);
        
        UpdateCastleState(kind, player, from);
        
        State.InvertPlayer();

        return outcome;
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

    public bool IsKingInCheck(Plane colour, int probePosition = -1)
    {
        var position = colour == Plane.White ? State.WhiteKingCell : State.BlackKingCell;

        if (probePosition > -1)
        {
            position = probePosition;
        }

        var opponentPlane = colour.InvertColour();
    
        var attacks = Moves[MoveSet.Knight][position];
        
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

        attacks = Moves[colour == Plane.White ? MoveSet.PawnWhiteAttack : MoveSet.PawnBlackAttack][position];
        
        if ((this[Plane.Pawn] & this[opponentPlane] & attacks) > 0)
        {
            return true;
        }

        attacks = Moves[MoveSet.King][position];

        if ((attacks & this[opponentPlane] & this[Plane.King]) > 0)
        {
            return true;
        }
        
        return false;
    }

    public void ParseFen(string fen)
    {
        var parts = fen.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 6)
        {
            throw new FenParseException($"Invalid number of parts to FEN string: {parts.Length}.");
        }

        var ranks = parts[0].Split('/');

        if (ranks.Length != Constants.Ranks)
        {
            throw new FenParseException($"Incorrect number of ranks in FEN string: {ranks.Length}.");
        }

        var whiteKingCell = 0;

        var blackKingCell = 0;

        var whiteScore = 0;

        var blackScore = 0;

        for (var rank = 0; rank < Constants.Ranks; rank++)
        {
            var files = ranks[Constants.MaxRank - rank];

            var file = 0;

            var index = 0;

            while (index < files.Length)
            {
                var cell = files[index];

                index++;
                
                if (char.IsNumber(cell))
                {
                    file += cell - '0';

                    if (file > Constants.Files)
                    {
                        throw new FenParseException($"Too many files in rank {rank + 1}: {files}.");
                    }

                    continue;
                }

                var colourPlane = char.IsUpper(cell) ? Plane.White : Plane.Black;
                
                var plane = char.ToUpper(cell) switch
                {
                    'P' => Plane.Pawn,
                    'R' => Plane.Rook,
                    'N' => Plane.Knight,
                    'B' => Plane.Bishop,
                    'Q' => Plane.Queen,
                    'K' => Plane.King,
                    _ => throw new FenParseException($"Invalid piece token in rank {rank + 1}: {cell}.")
                };

                var cellIndex = Cell.GetCell(rank, file);

                if (cellIndex < 0)
                {
                    throw new FenParseException($"Too many files in rank {rank + 1}: {files}.");
                }

                this[colourPlane] |= 1ul << cellIndex;

                this[plane] |= 1ul << cellIndex;
                
                if (plane == Plane.King)
                {
                    if (colourPlane == Plane.White)
                    {
                        whiteKingCell = cellIndex;
                    }
                    else
                    {
                        blackKingCell = cellIndex;
                    }
                }

                var piece = PieceCache.Get((Kind) plane);

                if (colourPlane == Plane.White)
                {
                    whiteScore += piece.Value;
                }
                else
                {
                    blackScore += piece.Value;
                }

                file++;
            }
            
            if (file != Constants.Files)
            {
                throw new FenParseException($"Not enough files in rank {rank + 1}: {files}.");
            }
        }
        
        var player = parts[1][0] switch
        {
            'b' => Colour.Black,
            'w' => Colour.White,
            _ => throw new FenParseException($"Invalid turn indicator: {parts[1][0]}.")
        };

        var castleAvailability = Castle.NotAvailable;
        
        if (parts[2] != "-")
        {
            foreach (var character in parts[2])
            {
                castleAvailability |= character switch
                {
                    'K' => Castle.WhiteKingSide,
                    'Q' => Castle.WhiteQueenSide,
                    'k' => Castle.BlackKingSide,
                    'q' => Castle.BlackQueenSide,
                    _ => throw new FenParseException($"Invalid castling status indicator: {character}.")
                };
            }
        }

        int? enPassantTarget = null;
        
        if (parts[3] != "-")
        {
            enPassantTarget = parts[3].FromStandardNotation();
        }

        if (! int.TryParse(parts[4], out var halfmoves))
        {
            throw new FenParseException($"Invalid value for halfmove counter: {parts[4]}.");
        }

        if (! int.TryParse(parts[5], out var fullmoves))
        {
            throw new FenParseException($"Invalid value for fullmove counter: {parts[5]}.");
        }

        State = new State(player, castleAvailability, enPassantTarget, whiteScore, blackScore, whiteKingCell, blackKingCell, halfmoves, fullmoves);
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