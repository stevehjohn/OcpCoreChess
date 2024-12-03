using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.General;

public static class AttackBitboards
{
    public static ulong[] KnightAttacks = new ulong[64];

    static AttackBitboards()
    {
        ComputeKnightAttacks();
    }

    private static void ComputeKnightAttacks()
    {
        for (var rank = 0; rank < Constants.Ranks; rank++)
        {
            for (var file = 0; file < Constants.Ranks; file++)
            {
                var pattern = 0ul;
                
                for (var i = 0; i < Constants.KnightMoves.Length; i++)
                {
                    var move = Constants.KnightMoves[i];

                    var target = Cell.GetCell(rank + move.RankDelta, file + move.FileDelta);

                    if (target < 0)
                    {
                        continue;
                    }

                    pattern |= 1ul << target;
                }

                KnightAttacks[Cell.GetCell(rank, file)] = pattern;
            }
        }
    }
}