using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Bitboards;

public class MoveCache
{
    private static readonly Lazy<MoveCache> Instantiator = new(Instantiate);
    
    private readonly MoveSet[] _moveSets;

    public static MoveCache Instance => Instantiator.Value;
    
    public MoveSet this[int cell] => _moveSets[cell];

    private MoveCache()
    {
        _moveSets = new MoveSet[Constants.Cells];

        for (var i = 0; i < Constants.Cells; i++)
        {
            _moveSets[i] = new MoveSet();
        }

        GeneratePawnMoves();
        
        GeneratePawnAttacks();

        GenerateOrthogonalMoves();
        
        GenerateDiagonalMoves();

        GenerateKnightMoves();
        
        GenerateKingMoves();
    }

    private static MoveCache Instantiate()
    {
        return new MoveCache();
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

            this[cell][MoveSets.PawnToBlack] = mask;

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

            this[cell][MoveSets.PawnToWhite] = mask;
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
            
            this[cell][MoveSets.PawnWhiteAttack] = mask;

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

            this[cell][MoveSets.PawnBlackAttack] = mask;
        }
    }

    private void GenerateOrthogonalMoves()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            this[cell][MoveSets.Horizontal] = GenerateHorizontalMoves(Cell.GetRank(cell));

            this[cell][MoveSets.Vertical] = GenerateVerticalMoves(Cell.GetFile(cell));
        }
    }

    private void GenerateDiagonalMoves()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            this[cell][MoveSets.Diagonal] = GenerateDiagonalMoves(cell);

            this[cell][MoveSets.AntiDiagonal] = GenerateAntiDiagonalMoves(cell);
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

            this[cell][MoveSets.Knight] = mask;
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

            this[cell][MoveSets.King] = mask;
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