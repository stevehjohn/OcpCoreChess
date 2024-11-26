using OcpCore.Engine.Extensions;
using Xunit;

namespace OcpCore.Engine.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("a8", 56)]
    [InlineData("h8", 63)]
    [InlineData("a1", 0)]
    [InlineData("h1", 7)]
    [InlineData("d5", 35)]
    [InlineData("e5", 36)]
    [InlineData("d4", 27)]
    [InlineData("e4", 28)]
    public void FromStandardNotationReturnsCorrectCell(string cell, int expectedCell)
    {
        Assert.Equal(cell.FromStandardNotation(), expectedCell);
    }
}