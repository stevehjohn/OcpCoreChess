using OcpCore.Engine.Extensions;
using Xunit;

namespace OcpCore.Engine.Tests.Extensions;

public class IntExtensionsTests
{
    [Theory]
    [InlineData(56, "a8")]
    [InlineData(63, "h8")]
    [InlineData(0, "a1")]
    [InlineData(7, "h1")]
    [InlineData(35, "d5")]
    [InlineData(36, "e5")]
    [InlineData(27, "d4")]
    [InlineData(28, "e4")]
    public void TotandardNotationReturnsCorrectCell(int cell, string expectedCell)
    {
        Assert.Equal(expectedCell, cell.ToStandardNotation());
    }

}