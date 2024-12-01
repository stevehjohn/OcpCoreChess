using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General.Bitboards;

public class Moves
{
    private readonly DirectionalMoves[] _moves;
    
    public DirectionalMoves this[Kind kind] => _moves[(int) kind];

    public Moves()
    {
        _moves = new DirectionalMoves[Constants.Pieces + 1];

        for (var i = 0; i <= Constants.Pieces; i++)
        {
            _moves[i] = new DirectionalMoves();
        }
        
        GeneratePawnMoves();

        GenerateRookMoves();

        GenerateKnightMoves();
        
        GenerateBishopMoves();
        
        GenerateQueenMoves();
        
        GenerateKingMoves();
    }

    private void GeneratePawnMoves()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            var mask = 0ul;

            var target = Cell.GetCell(Cell.GetRank(cell) + 1, Cell.GetFile(cell) + 1);

            if (target >= 0)
            {
                mask |= 1ul << target;
            }

            target = Cell.GetCell(Cell.GetRank(cell) + 1, Cell.GetFile(cell) - 1);

            if (target >= 0)
            {
                mask |= 1ul << target;
            }

            this[Kind.Pawn][MoveSet.ToBlack][cell] = mask;

            target = Cell.GetCell(Cell.GetRank(cell) - 1, Cell.GetFile(cell) + 1);

            if (target >= 0)
            {
                mask |= 1ul << target;
            }

            target = Cell.GetCell(Cell.GetRank(cell) - 1, Cell.GetFile(cell) - 1);

            if (target >= 0)
            {
                mask |= 1ul << target;
            }

            this[Kind.Pawn][MoveSet.ToWhite][cell] = mask;
        }
    }

    private void GenerateRookMoves()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            this[Kind.Rook][MoveSet.Horizontal][cell] = GenerateHorizontalMoves(Cell.GetRank(cell));

            this[Kind.Rook][MoveSet.Vertical][cell] = GenerateVerticalMoves(Cell.GetFile(cell));
        }
    }

    private void GenerateKnightMoves()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            var mask = 0ul;
            
            foreach (var move in Constants.KnightMoves)
            {
                var target = Cell.GetCell(Cell.GetRank(cell) + move.RankDelta, Cell.GetFile(cell) + move.FileDelta);

                if (target >= 0)
                {
                    mask |= 1ul << target;
                }
            }

            this[Kind.Knight][MoveSet.Specific][cell] = mask;
        }
    }

    private void GenerateBishopMoves()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            this[Kind.Bishop][MoveSet.Diagonal][cell] = GenerateDiagonalMoves(cell);

            this[Kind.Bishop][MoveSet.AntiDiagonal][cell] = GenerateAntiDiagonalMoves(cell);
        }
    }

    private void GenerateQueenMoves()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            this[Kind.Queen][MoveSet.Horizontal][cell] = GenerateHorizontalMoves(Cell.GetRank(cell));

            this[Kind.Queen][MoveSet.Vertical][cell] = GenerateVerticalMoves(Cell.GetFile(cell));

            this[Kind.Queen][MoveSet.Diagonal][cell] = GenerateDiagonalMoves(cell);

            this[Kind.Queen][MoveSet.AntiDiagonal][cell] = GenerateAntiDiagonalMoves(cell);
        }
    }

    private void GenerateKingMoves()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            var mask = 0ul;
            
            foreach (var move in Constants.DirectionalMoves)
            {
                var target = Cell.GetCell(Cell.GetRank(cell) + move.RankDelta, Cell.GetFile(cell) + move.FileDelta);

                if (target >= 0)
                {
                    mask |= 1ul << target;
                }
            }

            this[Kind.King][MoveSet.Specific][cell] = mask;
        }
    }
    
    private static ulong GenerateHorizontalMoves(int rank)
    {
        return 0b1111_1111ul << (rank * 8);
    }
    
    private static ulong GenerateVerticalMoves(int file)
    {
        return 0b0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001ul << file;
    }

    private static ulong GenerateDiagonalMoves(int cell)
    {
        var rank = Cell.GetRank(cell);
        
        var file = Cell.GetFile(cell);
        
        var index = rank - file;

        var mask = 0ul;
        
        for (var r = 0; r < Constants.Ranks; r++)
        {
            for (var f = 0; f < Constants.Files; f++)
            {
                if (r - f == index)
                {
                    mask |= 1ul << (r * 8 + f);
                }
            }
        }
        return mask; 
    }

    private static ulong GenerateAntiDiagonalMoves(int cell)
    {
        if (cell == 62)
        {
        }

        var rank = Cell.GetRank(cell);
        
        var file = Cell.GetFile(cell);
        
        var index = rank + file;

        var mask = 0ul;
        
        for (var r = 0; r < Constants.Ranks; r++)
        {
            for (var f = 0; f < Constants.Files; f++)
            {
                if (r + f == index)
                {
                    mask |= 1ul << (r * 8 + f);
                }
            }
        }
        return mask;
    }
}