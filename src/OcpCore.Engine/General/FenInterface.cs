using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.General;

public static class FenInterface
{
    public static State ParseFen(string fen, ulong[] planes)
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

                planes[(int) colourPlane] |= 1ul << cellIndex;

                planes[(int) plane] |= 1ul << cellIndex;
                
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

        return new State(player, castleAvailability, enPassantTarget, whiteScore, blackScore, whiteKingCell, blackKingCell, halfmoves, fullmoves);
    }
}