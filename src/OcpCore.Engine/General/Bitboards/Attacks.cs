using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General.Bitboards;

public class Attacks
{
    private readonly ulong[][][] _attacks;
    
    public ulong[][] this[Kind kind] => _attacks[(int) kind];

    public Attacks()
    {
        _attacks = new ulong[Constants.Pieces][][];

        for (var i = 0; i < Constants.Pieces; i++)
        {
            _attacks[i] = new ulong[Constants.Cells][];
        }

        GenerateQueenAttacks();
    }

    private void GenerateQueenAttacks()
    {
        for (var cell = 0; cell < Constants.Cells; cell++)
        {
            this[Kind.Queen][cell] = new ulong[4];

            this[Kind.Queen][cell][(int) Direction.Horizontal] = GenerateHorizontalAttacks(Cell.GetRank(cell));
            
            this[Kind.Queen][cell][(int) Direction.Vertical] = GenerateVerticalAttacks(Cell.GetFile(cell));
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
}