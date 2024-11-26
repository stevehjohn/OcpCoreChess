namespace OcpCore.Engine.Extensions;

public static class StringExtensions
{
    public static int FromStandardNotation(this string cell)
    {
        var file = cell[0] - 'a';

        var rank = cell[1] - '1';

        return rank * 8 + file;
    }
}