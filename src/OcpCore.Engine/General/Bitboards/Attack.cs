using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General.Bitboards;

public class Attack
{
    private readonly ulong[][][] _attacks;
    
    public ulong[][] this[Kind kind] => _attacks[(int) kind];

    public Attack()
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
            var attacks = this[Kind.Queen][cell];

            attacks = new ulong[4];

            attacks[0] = GenerateHorizontalAttacks(Cell.GetRank(cell));
        }
    }

    private static ulong GenerateHorizontalAttacks(int rank)
    {
        return 0b1111_1111ul << ((rank - 1) * 8);
    }
}