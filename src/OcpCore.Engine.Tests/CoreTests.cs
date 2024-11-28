using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests;

public class CoreTests
{
    [Theory]
    [InlineData(1, 20, 0, 0, 0, 0, 0, 0)]
    [InlineData(2, 400, 0, 0, 0, 0, 0, 0)]
    [InlineData(3, 8_902, 34, 0, 0, 0, 12, 0)]
    [InlineData(4, 197_281, 1_576, 0, 0, 0, 469, 8)]
    [InlineData(5, 4_865_609, 82_719, 258, 0, 0, 27_351, 347)]
    public void ReturnsExpectedCountAtPly(int ply, int count, int capture, int enPassant, int castle, int promotion, int check, int mate)
    {
        using var core = new Core(Colour.White);
    
        core.GetMove(ply);
        
        Assert.Equal(count, core.GetDepthCount(ply));
        
        Assert.Equal(capture, core.GetMoveOutcome(ply, MoveOutcome.Capture));
        
        Assert.Equal(enPassant, core.GetMoveOutcome(ply, MoveOutcome.EnPassant));
        
        Assert.Equal(castle, core.GetMoveOutcome(ply, MoveOutcome.Castle));
        
        Assert.Equal(promotion, core.GetMoveOutcome(ply, MoveOutcome.Promotion));
        
        Assert.Equal(check, core.GetMoveOutcome(ply, MoveOutcome.Check));
        
        Assert.Equal(mate, core.GetMoveOutcome(ply, MoveOutcome.CheckMate));
    }
}