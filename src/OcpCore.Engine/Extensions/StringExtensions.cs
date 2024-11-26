namespace OcpCore.Engine.Extensions;

public static class StringExtensions
{
    public static int FromStandardNotation(this string cell)
    {
        var file = cell[0] - 'a';

        var rank = 8 - (cell[1] - '0');

        return rank * 8 + file;
    }
}