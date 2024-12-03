using System.Numerics;
using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

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

        var colour = (this[Plane.White] & fromBit) == fromBit ? Colour.White : Colour.Black;

        var kind = GetKindInternal(fromBit);

        if (kind == Kind.King)
        {
            if (colour == Colour.White)
            {
                State.SetWhiteKingCell(to);
            }
            else
            {
                State.SetBlackKingCell(to);
            }
        }

        var outcome = MoveOutcome.Move;

        if (IsColour(colour.Invert(), to))
        {
            outcome |= MoveOutcome.Capture;
        }
        
        UpdateBitboards(kind, colour, fromBit, toBit);

        State.InvertPlayer();

        return outcome;
    }

    public bool IsKind(Kind kind, int cell)
    {
        return kind switch
        {
            Kind.Pawn => (this[Plane.Pawn] & (1ul << cell)) > 0,
            Kind.Rook => (this[Plane.Rook] & (1ul << cell)) > 0,
            Kind.Knight => (this[Plane.Knight] & (1ul << cell)) > 0,
            Kind.Bishop => (this[Plane.Bishop] & (1ul << cell)) > 0,
            Kind.Queen => (this[Plane.Queen] & (1ul << cell)) > 0,
            Kind.King => (this[Plane.King] & (1ul << cell)) > 0,
            _ => false
        };
    }

    public bool IsEmpty(int cell)
    {
        return ((this[Plane.White] | this[Plane.Black]) & (1ul << cell)) == 0;
    }

    public bool IsColour(Colour colour, int cell)
    {
        return colour == Colour.White 
            ? (this[Plane.White] & (1ul << cell)) > 0 
            : (this[Plane.Black] & (1ul << cell)) > 0;
    }

    public Kind GetKind(int cell)
    {
        return GetKindInternal(1ul << cell);
    }

    public bool IsKingInCheck(Colour colour)
    {
        var position = colour == Colour.White ? State.WhiteKingCell : State.BlackKingCell;
    
        var opponentColor = colour == Colour.White ? Plane.Black : Plane.White;
    
        var attacks = Moves[MoveSet.Knight][position];
        
        if ((attacks & this[opponentColor]) > 0)
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

        // TODO: Scores
        State = new State(player, castleAvailability, enPassantTarget, 0, 0, whiteKingCell, blackKingCell, halfmoves, fullmoves);
    }

    private Kind GetKindInternal(ulong cellBit)
    {
        if ((this[Plane.Pawn] & cellBit) == cellBit)
        {
            return Kind.Pawn;
        }
        
        if ((this[Plane.Rook] & cellBit) == cellBit)
        {
            return Kind.Rook;
        }
        
        if ((this[Plane.Knight] & cellBit) == cellBit)
        {
            return Kind.Knight;
        }
        
        if ((this[Plane.Bishop] & cellBit) == cellBit)
        {
            return Kind.Bishop;
        }
        
        if ((this[Plane.Queen] & cellBit) == cellBit)
        {
            return Kind.Queen;
        }
        
        if ((this[Plane.King] & cellBit) == cellBit)
        {
            return Kind.King;
        }

        throw new InvalidMoveException($"No piece at {BitOperations.TrailingZeroCount(cellBit).ToStandardNotation()}.");
    }

    private void UpdateBitboards(Kind kind, Colour colour, ulong fromBit, ulong toBit)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (colour)
        {
            case Colour.White:
                this[Plane.White] &= ~fromBit;
                this[Plane.White] |= toBit;
                
                this[Plane.Black] &= ~toBit;
                break;

            case Colour.Black:
                this[Plane.Black] &= ~fromBit;
                this[Plane.Black] |= toBit;
                
                this[Plane.White] &= ~toBit;
                break;
        }

        this[Plane.Pawn] &= ~toBit;
        this[Plane.Rook] &= ~toBit;
        this[Plane.Knight] &= ~toBit;
        this[Plane.Bishop] &= ~toBit;
        this[Plane.Rook] &= ~toBit;
        this[Plane.Rook] &= ~toBit;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (kind)
        {
            case Kind.Pawn:
                this[Plane.Pawn] &= ~fromBit;
                this[Plane.Pawn] |= toBit;
                break;

            case Kind.Rook:
                this[Plane.Rook] &= ~fromBit;
                this[Plane.Rook] |= toBit;
                break;

            case Kind.Knight:
                this[Plane.Knight] &= ~fromBit;
                this[Plane.Knight] |= toBit;
                break;

            case Kind.Bishop:
                this[Plane.Bishop] &= ~fromBit;
                this[Plane.Bishop] |= toBit;
                break;

            case Kind.Queen:
                this[Plane.Queen] &= ~fromBit;
                this[Plane.Queen] |= toBit;
                break;

            case Kind.King:
                this[Plane.King] &= ~fromBit;
                this[Plane.King] |= toBit;
                break;
        }
    }
}