using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Extensions;

public static class IntExtensions
{
    public static string ToStandardNotation(this int cell)
    {
        return $"{(char) ('a' + (cell & Masks.File))}{(cell >> Offsets.Rank) + 1}";
    }
}