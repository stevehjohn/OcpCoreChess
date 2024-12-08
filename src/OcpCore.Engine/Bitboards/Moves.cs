using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Bitboards;

public class Moves
{
    private static readonly Lazy<Moves> Instantiator = new(Instantiate);
    
    private readonly ulong[][] _moveSets;

    public static Moves Instance => Instantiator.Value;
    
    public ulong[] this[MoveSet moveSet] => _moveSets[(int) moveSet];

    private Moves()
    {
        var sets = Enum.GetValues<MoveSet>().Length;

        _moveSets = new ulong[sets + 1][];
        
        for (var i = 0; i < sets; i++)
        {
            _moveSets[i] = new ulong[Constants.Cells];
        }

        GeneratePawnMoves();
        
        GeneratePawnAttacks();

        GenerateOrthogonalMoves();
        
        GenerateDiagonalMoves();

        GenerateKnightMoves();
        
        GenerateKingMoves();
    }

    private static Moves Instantiate()
    {
        return new Moves();
    }

    private void GeneratePawnMoves()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            var mask = 0ul;

            var target = Cell.GetCell(Cell.GetRank(cell) + 1, Cell.GetFile(cell));

            if (target >= 0)
            {
                mask |= 1ul << target;
            }

            if (Cell.GetRank(cell) == Ranks.WhitePawn)
            {
                target += 8;
                
                mask |= 1ul << target;
            }

            this[MoveSet.PawnToBlack][cell] = mask;

            mask = 0ul;

            target = Cell.GetCell(Cell.GetRank(cell) - 1, Cell.GetFile(cell));

            if (target >= 0)
            {
                mask |= 1ul << target;
            }

            if (Cell.GetRank(cell) == Ranks.BlackPawn)
            {
                target -= 8;
                
                mask |= 1ul << target;
            }

            this[MoveSet.PawnToWhite][cell] = mask;
        }
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
            
            this[MoveSet.PawnWhiteAttack][cell] = mask;

            mask = 0ul;

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

            this[MoveSet.PawnBlackAttack][cell] = mask;
        }
    }

    private void GenerateOrthogonalMoves()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            this[MoveSet.Horizontal][cell] = GenerateHorizontalMoves(Cell.GetRank(cell));

            this[MoveSet.Vertical][cell] = GenerateVerticalMoves(Cell.GetFile(cell));
        }
    }

    private void GenerateDiagonalMoves()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            this[MoveSet.Diagonal][cell] = GenerateDiagonalMoves(cell);

            this[MoveSet.AntiDiagonal][cell] = GenerateAntiDiagonalMoves(cell);
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

            this[MoveSet.Knight][cell] = mask;
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

            this[MoveSet.King][cell] = mask;
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