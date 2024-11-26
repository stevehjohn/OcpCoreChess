using System.Runtime.CompilerServices;

namespace OcpCore.Engine.General;

public static class Cell
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetRank(int cell)
    {
        return cell >> Constants.RankOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetFile(int cell)
    {
        return cell & Constants.FileMask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetCell(int rank, int file)
    {
        if (rank is < 0 or > Constants.MaxRank || file < 0 || file > Constants.MaxFile)
        {
            return -1;
        }

        return (rank << Constants.RankOffset) | file;
    }
}