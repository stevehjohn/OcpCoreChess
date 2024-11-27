using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Extensions;
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

        State = new State(Colour.White, Castle.WhiteQueenSide | Castle.WhiteKingSide | Castle.BlackQueenSide | Castle.BlackKingSide, 0);
    }

    public Board(string fen)
    {
        _cells = new byte[Constants.Cells];
        
        ParseFen(fen);
    }

    public void MakeMove(int position, int target)
    {
        var piece = _cells[position];
        
        _cells[target] = piece;

        _cells[position] = 0;

        CheckCastlingRightsForKing(piece);

        CheckCastlingRightsForRook(piece, position);

        UpdateEnPassantState(piece, position, target);
    }

    private void CheckCastlingRightsForKing(byte piece)
    {
        if (Cell.Is(piece, Kind.King))
        {
            var colour = Cell.Colour(piece); 

            switch (colour)
            {
                case Colour.White:
                    State.RemoveCastleRights(Castle.WhiteKingSide);
                    State.RemoveCastleRights(Castle.WhiteQueenSide);
                    
                    break;

                case Colour.Black:
                    State.RemoveCastleRights(Castle.BlackKingSide);
                    State.RemoveCastleRights(Castle.BlackQueenSide);
                    
                    break;
            }
        }
    }
    
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

    private void UpdateEnPassantState(byte piece, int position, int target)
    {
        if (Cell.Is(piece, Kind.Pawn))
        {
            var delta = position - target;

            if (Math.Abs(delta) == Constants.Ranks * 2)
            {
                State.SetEnPassantTarget(delta > 0 ? position - Constants.Files : position + Constants.Files);
            }
            else
            {
                State.SetEnPassantTarget(null);
            }
        }
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

        for (var rank = 0; rank < Constants.Ranks; rank++)
        {
            var files = ranks[Constants.MaxRank - rank];

            var file = 0;

            var index = 0;

            while (index < files.Length)
            {
                if (index >= files.Length)
                {
                    throw new FenParseException($"Not enough files in rank {rank + 1}: {files}.");
                }

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

                var kind = char.ToUpper(cell) switch
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

                _cells[cellIndex] = (byte) kind;

                file++;
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
                    _ => throw new FenParseException($"Invalid castling status indicator: {character}")
                };
            }
        }

        var enPassantTarget = 0;
        
        if (parts[3] != "-")
        {
            enPassantTarget = parts[3].FromStandardNotation();
        }

        State = new State(player, castleAvailability, enPassantTarget);
    }
}