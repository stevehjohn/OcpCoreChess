using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests;

public class CoreTests
{
    [Theory]
    // [InlineData(1, 20)]
    // [InlineData(2, 400)]
    // [InlineData(3, 8_902)]
    // [InlineData(4, 197_281)]
    [InlineData(5, 4_865_609)]
    public void ReturnsExpectedCountAtPly(int ply, int expectedCount)
    {
        var core = new Core(Colour.White);
    
        core.GetMove(ply);
        
        Assert.Equal(expectedCount, core.GetDepthCount(ply));
    }
}