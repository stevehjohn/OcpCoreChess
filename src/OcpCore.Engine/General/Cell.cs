using System.Runtime.CompilerServices;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.General;

public static class Cell
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetRank(int cell)
    {
        return cell >> Offsets.Rank;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetFile(int cell)
    {
        return cell & Masks.File;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetCell(int rank, int file)
    {
        if (rank is < 0 or > Constants.MaxRank || file is < 0 or > Constants.MaxFile)
        {
            return -1;
        }

        return (rank << Offsets.Rank) | file;
    }
}