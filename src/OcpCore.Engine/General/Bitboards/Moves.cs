using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General.Bitboards;

public class Moves
{
    private readonly DirectionalMoves[] _attacks;
    
    public DirectionalMoves this[Kind kind] => _attacks[(int) kind];

    public Moves()
    {
        _attacks = new DirectionalMoves[Constants.Pieces + 1];

        for (var i = 0; i <= Constants.Pieces; i++)
        {
            _attacks[i] = new DirectionalMoves();
        }
        
        GeneratePawnAttacks();

        GenerateRookAttacks();

        GenerateKnightAttacks();
        
        GenerateBishopAttacks();
        
        GenerateQueenAttacks();
        
        GenerateKingAttacks();
    }

    private void GeneratePawnAttacks()
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

    private void GenerateRookAttacks()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            this[Kind.Rook][MoveSet.Horizontal][cell] = GenerateHorizontalAttacks(Cell.GetRank(cell));

            this[Kind.Rook][MoveSet.Vertical][cell] = GenerateVerticalAttacks(Cell.GetFile(cell));
        }
    }

    private void GenerateKnightAttacks()
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

    private void GenerateBishopAttacks()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            this[Kind.Bishop][MoveSet.Diagonal][cell] = GenerateDiagonalAttacks(cell);

            this[Kind.Bishop][MoveSet.AntiDiagonal][cell] = GenerateAntiDiagonalAttacks(cell);
        }
    }

    private void GenerateQueenAttacks()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            this[Kind.Queen][MoveSet.Horizontal][cell] = GenerateHorizontalAttacks(Cell.GetRank(cell));

            this[Kind.Queen][MoveSet.Vertical][cell] = GenerateVerticalAttacks(Cell.GetFile(cell));

            this[Kind.Queen][MoveSet.Diagonal][cell] = GenerateDiagonalAttacks(cell);

            this[Kind.Queen][MoveSet.AntiDiagonal][cell] = GenerateAntiDiagonalAttacks(cell);
        }
    }

    private void GenerateKingAttacks()
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
    
    private static ulong GenerateHorizontalAttacks(int rank)
    {
        return 0b1111_1111ul << (rank * 8);
    }
    
    private static ulong GenerateVerticalAttacks(int file)
    {
        return 0b0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001ul << file;
    }

    private static ulong GenerateDiagonalAttacks(int cell)
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

    private static ulong GenerateAntiDiagonalAttacks(int cell)
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