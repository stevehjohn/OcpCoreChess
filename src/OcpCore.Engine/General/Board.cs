using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public class Board
{
    private readonly byte[] _cells;

    public byte this[int index] => _cells[index];

    public Board()
    {
        _cells = new byte[Constants.Cells];
    }

    public Board(string fen)
    {
        _cells = new byte[Constants.Cells];
        
        ParseFen(fen);
    }

    private void ParseFen(string fen)
    {
        var parts = fen.Split(' ');

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
                    throw new FenParseException($"Not enough files in rank {files}.");
                }

                var cell = files[index];

                index++;
                
                if (char.IsNumber(cell))
                {
                    file += cell - '0';

                    if (file > Constants.Files)
                    {
                        throw new FenParseException($"Too many files in rank: {files}.");
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
                    _ => throw new FenParseException($"Invalid piece token: {cell}.")
                };

                var cellIndex = Cell.GetCell(rank, file);

                if (cellIndex < 0)
                {
                    throw new FenParseException($"Too many files in rank: {files}.");
                }

                _cells[cellIndex] = (byte) kind;

                file++;
            }
        }
    }
}