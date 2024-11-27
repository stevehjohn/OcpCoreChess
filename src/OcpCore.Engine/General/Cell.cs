using System.Runtime.CompilerServices;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.General;

public static class Cell
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetRank(int cell)
    {
        return cell >> Offsets.RankOffset;
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

        return (rank << Offsets.RankOffset) | file;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is(byte piece, Kind kind)
    {
        return (piece & Masks.Kind) == (byte) kind;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Colour Colour(byte piece)
    {
        return (Colour) (piece & Masks.Colour);
    }

    public static Kind Kind(byte piece)
    {
        return (Kind) (piece & Masks.File);
    }
}