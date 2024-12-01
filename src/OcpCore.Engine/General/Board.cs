using System.Runtime.CompilerServices;
using System.Text;
using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public class Board
{
    private readonly byte[] _cells;

    public byte this[int index] => _cells[index];

    public State State { get; private set; }

    public Board()
    {
        _cells = new byte[Constants.Cells];

        State = new State(Colour.White, Castle.WhiteQueenSide | Castle.WhiteKingSide | Castle.BlackQueenSide | Castle.BlackKingSide, 0, 0, 0, 0, 0, 0, 1);
    }

    public Board(string fen)
    {
        _cells = new byte[Constants.Cells];
        
        ParseFen(fen);
    }

    public unsafe Board(Board board)
    {
        _cells = new byte[Constants.Cells];
        
        fixed (byte* destination = _cells)
        {
            fixed (byte* source = board._cells)
            {
                Buffer.MemoryCopy(source, destination, Constants.Cells, Constants.Cells);
            }
        }

        State = new State(board.State);
    }

    public MoveOutcome MakeMove(int position, int target)
    {
        var outcome = MoveOutcome.Move;

        var piece = _cells[position];

        if (piece == 0 || Cell.Colour(piece) != State.Player)
        {
            throw new InvalidMoveException(piece == 0 ? $"No piece at position {position.ToStandardNotation()}." : $"Not the turn for {Cell.Colour(piece)}.");
        }

        var capture = _cells[target];

        if (capture != 0)
        {
            var score = PieceCache.Get(capture).Value;

            if (Cell.Colour(capture) == Colour.White)
            {
                State.UpdateWhiteScore(-score);
            }
            else
            {
                State.UpdateBlackScore(-score);
            }

            outcome |= MoveOutcome.Capture;
        }

        _cells[target] = piece;

        if (Cell.Is(piece, Kind.King))
        {
            if (Cell.Colour(piece) == Colour.White)
            {
                State.SetWhiteKingCell(target);
            }
            else
            {
                State.SetBlackKingCell(target);
            }
        }

        _cells[position] = 0;

        outcome |= PerformCastle(piece, position, target);

        outcome |= PerformEnPassant(piece, target);

        CheckCastlingRightsForKing(piece);

        CheckCastlingRightsForRook(piece, position);

        UpdateEnPassantState(piece, position, target);

        outcome |= CheckForPromotion(piece, target);

        if (State.Player == Colour.Black)
        {
            State.IncrementFullmoves();
        }

        if (Cell.Is(piece, Kind.Pawn) || (outcome & MoveOutcome.Capture) > 0)
        {
            State.ResetHalfmoves();
        }
        else
        {
            State.IncrementHalfmoves();
        }

        State.InvertPlayer();

        return outcome;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MoveOutcome PerformCastle(byte piece, int position, int target)
    {
        var delta = position - target;

        if (Cell.Is(piece, Kind.King) && Math.Abs(delta) == 2)
        {
            var rank = Cell.GetRank(position);

            var file = delta > 0 ? Files.LeftRook : Files.RightRook;

            var targetFile = delta > 0 ? Files.Queen : Files.RightBishop;

            var sourceFile = Cell.GetCell(rank, file);

            _cells[Cell.GetCell(rank, targetFile)] = _cells[sourceFile];

            _cells[sourceFile] = 0;

            return MoveOutcome.Castle;
        }

        return MoveOutcome.Move;
    }

    public bool IsKingInCheck(Colour player, int probeCell = -1)
    {
        var kingCell = player == Colour.White ? State.WhiteKingCell : State.BlackKingCell;

        if (probeCell > -1)
        {
            kingCell = probeCell;
        }

        var kingRank = Cell.GetRank(kingCell);

        var kingFile = Cell.GetFile(kingCell);

        int cellRank;

        int cellFile;

        int cell;

        byte piece;
        
        for (var d = 0; d < Constants.DirectionalMovesAnnotated.Length; d++)
        {
            var direction = Constants.DirectionalMovesAnnotated[d];
            
            cellRank = kingRank;

            cellFile = kingFile;
            
            for (var i = 0; i < Constants.MaxMoveDistance; i++)
            {
                cellRank += direction.RankDelta;

                cellFile += direction.FileDelta;

                cell = Cell.GetCell(cellRank, cellFile);

                if (cell < 0)
                {
                    break;
                }

                piece = _cells[cell];

                if (piece > 0 && Cell.Colour(piece) == player)
                {
                    break;
                }

                var kind = Cell.Kind(piece);

                var isAttacking = kind switch
                {
                    Kind.Queen => true,
                    Kind.Rook => direction.IsOrthogonal,
                    Kind.Bishop => ! direction.IsOrthogonal,
                    Kind.King => i == 0,
                    _ => false
                };

                if (isAttacking)
                {
                    return true;
                }

                if (piece != 0)
                {
                    break;
                }
            }
        }
        
        for (var d = 0; d < Constants.KnightMoves.Length; d++)
        {
            var direction = Constants.KnightMoves[d];

            cellRank = kingRank;

            cellFile = kingFile;

            cellRank += direction.RankDelta;

            cellFile += direction.FileDelta;

            cell = Cell.GetCell(cellRank, cellFile);
        
            if (cell < 0)
            {
                continue;
            }
            
            piece = _cells[cell];

            if (piece > 0 && Cell.Colour(piece) == player)
            {
                continue;
            }
        
            if (Cell.Is(piece, Kind.Knight))
            {
                return true;
            }
        }
        
        var rankDirection = player == Colour.White ? Direction.White : Direction.Black;
        
        cell = Cell.GetCell(kingRank + rankDirection, kingFile - 1);
        
        if (cell >= 0)
        {
            piece = _cells[cell];
        
            if (piece > 0 && Cell.Colour(piece) == player.Invert() && Cell.Is(piece, Kind.Pawn))
            {
                return true;
            }
        }
        
        cell = Cell.GetCell(kingRank + rankDirection, kingFile + 1);
        
        if (cell >= 0)
        {
            piece = _cells[cell];
        
            if (piece > 0 && Cell.Colour(piece) == player.Invert() && Cell.Is(piece, Kind.Pawn))
            {
                return true;
            }
        }
        
        return false;
    }

#pragma warning disable CS8524 // Cannot happen
    public string Fen()
    {
        var builder = new StringBuilder();
        
        for (var rank = Constants.Ranks - 1; rank >= 0; rank--)
        {
            var blanks = 0;
            
            for (var file = 0; file < Constants.Files; file++)
            {
                var piece = _cells[Cell.GetCell(rank, file)];

                if (piece == 0)
                {
                    blanks++;
                    
                    continue;
                }

                if (blanks > 0)
                {
                    builder.Append(blanks);
                }

                blanks = 0;

                var character = Cell.Kind(piece) switch
                {
                    Kind.Pawn => 'P',
                    Kind.Rook => 'R',
                    Kind.Knight => 'N',
                    Kind.Bishop => 'B',
                    Kind.Queen => 'Q',
                    Kind.King => 'K'
                };

                if (Cell.Colour(piece) == Colour.Black)
                {
                    character = char.ToLower(character);
                }

                builder.Append(character);
            }

            if (blanks != 0)
            {
                builder.Append(blanks);
            }

            if (rank > 0)
            {
                builder.Append('/');
            }
        }

        builder.Append($" {(State.Player == Colour.White ? 'w' : 'b')} ");

        if (State.CastleStatus == Castle.NotAvailable)
        {
            builder.Append('-');
        }
        else
        {
            builder.Append((State.CastleStatus & Castle.WhiteKingSide) > 0 ? "K" : string.Empty);
            builder.Append((State.CastleStatus & Castle.WhiteQueenSide) > 0 ? "Q" : string.Empty);
            builder.Append((State.CastleStatus & Castle.BlackKingSide) > 0 ? "k" : string.Empty);
            builder.Append((State.CastleStatus & Castle.BlackQueenSide) > 0 ? "q" : string.Empty);
        }

        if (State.EnPassantTarget == null)
        {
            builder.Append(" -");
        }
        else
        {
            builder.Append($" {State.EnPassantTarget.Value.ToStandardNotation()}");
        }

        builder.Append($" {State.Halfmoves}");

        builder.Append($" {State.Fullmoves}");

        return builder.ToString();
    }
#pragma warning restore CS8524

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MoveOutcome PerformEnPassant(byte piece, int target)
    {
        if (Cell.Is(piece, Kind.Pawn) && target == State.EnPassantTarget)
        {
            var colour = Cell.Colour(piece);

            var direction = colour == Colour.White ? Direction.Black : Direction.White;

            var score = PieceCache.Get(_cells[target + direction * Constants.Files]).Value;

            if (Cell.Colour(piece) == Colour.White)
            {
                State.UpdateWhiteScore(-score);
            }
            else
            {
                State.UpdateBlackScore(-score);
            }
            
            _cells[target + direction * Constants.Files] = 0;
            
            return MoveOutcome.EnPassant | MoveOutcome.Capture;
        }

        return MoveOutcome.Move;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckCastlingRightsForKing(byte piece)
    {
        if (Cell.Is(piece, Kind.King))
        {
            var colour = Cell.Colour(piece); 

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault - default can't happen
            switch (colour)
            {
                case Colour.White:
                    State.RemoveCastleRights(Castle.White);
                    
                    break;

                case Colour.Black:
                    State.RemoveCastleRights(Castle.Black);
                    
                    break;
            }
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckCastlingRightsForRook(byte piece, int position)
    {
        if (Cell.Is(piece, Kind.Rook))
        {
            var colour = Cell.Colour(piece); 

            switch (position, colour)
            {
                case (Files.LeftRook, Colour.White):
                    State.RemoveCastleRights(Castle.WhiteQueenSide);
                    
                    break;

                case (Files.RightRook, Colour.White):
                    State.RemoveCastleRights(Castle.WhiteKingSide);
                    
                    break;
                
                case (Constants.BlackRankCellStart + Files.LeftRook, Colour.Black):
                    State.RemoveCastleRights(Castle.BlackQueenSide);
                    
                    break;

                case (Constants.BlackRankCellStart + Files.RightRook, Colour.Black):
                    State.RemoveCastleRights(Castle.BlackKingSide);
                    
                    break;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateEnPassantState(byte piece, int position, int target)
    {
        if (Cell.Is(piece, Kind.Pawn))
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

    private MoveOutcome CheckForPromotion(byte piece, int target)
    {
        if (! Cell.Is(piece, Kind.Pawn))
        {
            return MoveOutcome.Move;
        }

        var rank = Cell.GetRank(target);

        if (rank is not (Ranks.BlackHomeRank or Ranks.WhiteHomeRank))
        {
            return MoveOutcome.Move;
        }
        
        // TODO: Knight sometimes?
        _cells[target] |= (byte) ((_cells[target] & ~Masks.Kind) | (byte) Kind.Queen);

        if (Cell.Colour(piece) == Colour.White)
        {
            State.UpdateWhiteScore(Scores.Queen - Scores.Pawn);
        }
        else
        {
            State.UpdateBlackScore(Scores.Queen - Scores.Pawn);
        }

        return MoveOutcome.Promotion;
    }
    
    private void ParseFen(string fen)
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

                var colour = char.IsUpper(cell) ? Colour.White : Colour.Black;

                var piece = char.ToUpper(cell) switch
                {
                    'P' => (byte) Kind.Pawn | (byte) colour,
                    'R' => (byte) Kind.Rook | (byte) colour,
                    'N' => (byte) Kind.Knight | (byte) colour,
                    'B' => (byte) Kind.Bishop | (byte) colour,
                    'Q' => (byte) Kind.Queen | (byte) colour,
                    'K' => (byte) Kind.King | (byte) colour,
                    _ => throw new FenParseException($"Invalid piece token in rank {rank + 1}: {cell}.")
                };

                var cellIndex = Cell.GetCell(rank, file);

                if (cellIndex < 0)
                {
                    throw new FenParseException($"Too many files in rank {rank + 1}: {files}.");
                }

                _cells[cellIndex] = (byte) piece;

                if (Cell.Is((byte) piece, Kind.King))
                {
                    if (colour == Colour.White)
                    {
                        whiteKingCell = cellIndex;
                    }
                    else
                    {
                        blackKingCell = cellIndex;
                    }
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

        var scores = CalculateScores();
        
        State = new State(player, castleAvailability, enPassantTarget, scores.White, scores.Black, whiteKingCell, blackKingCell, halfmoves, fullmoves);
    }

    private (int White, int Black) CalculateScores()
    {
        var white = 0;

        var black = 0;
        
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            var content = _cells[cell];
            
            if (content == 0)
            {
                continue;
            }

            if (Cell.Colour(content) == Colour.White)
            {
                white += PieceCache.Get(content).Value;
            }
            else
            {
                black += PieceCache.Get(content).Value;
            }
        }

        return (white, black);
    }

#pragma warning disable CS8524 // Cannot happen
    public override string ToString()
    {
        var builder = new StringBuilder();
        
        for (var rank = Constants.MaxRank; rank >= 0; rank--)
        {
            for (var file = 0; file < Constants.Files; file++)
            {
                var cellIndex = Cell.GetCell(rank, file);

                var piece = _cells[cellIndex];
                
                if (piece == 0)
                {
                    builder.Append(' ');
                    
                    continue;
                }

                var character = Cell.Kind(piece) switch
                {
                    Kind.Pawn => 'P',
                    Kind.Rook => 'R',
                    Kind.Knight => 'N',
                    Kind.Bishop => 'B',
                    Kind.Queen => 'Q',
                    Kind.King => 'K'
                };

                if (Cell.Colour(piece) == Colour.Black)
                {
                    character = char.ToLower(character);
                }

                builder.Append(character);
            }

            if (rank > 0)
            {
                builder.AppendLine();
            }
        }

        return builder.ToString();
    }
#pragma warning restore CS8524
}