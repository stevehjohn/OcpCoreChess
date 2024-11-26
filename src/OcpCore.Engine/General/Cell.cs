using System.Runtime.CompilerServices;
using OcpCore.Engine.Pieces;

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
        return cell & Masks.File;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is(int piece, Kind kind)
    {
        return (piece & (byte) kind) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Colour Colour(int piece)
    {
        return (Colour) (piece & Masks.Colour);
    }
}