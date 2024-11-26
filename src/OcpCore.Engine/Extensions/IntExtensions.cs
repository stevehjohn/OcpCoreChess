namespace OcpCore.Engine.Extensions;

public static class IntExtensions
{
    public static string ToStandardNotation(this int cell)
    {
        return $"{(char) ('a' + (cell & 7))}{8 - (cell >> 3)}";
    }
}