using OcpCore.Engine.General;
using OcpCore.Engine.General.Bitboards;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.General.Bitboards;

public class AttacksTests
{
    private readonly Attacks _attacks = new();
    
    [Theory]
    [InlineData(0, "0000000000000000000000000000000000000000000000000000000011111111")]
    [InlineData(7, "0000000000000000000000000000000000000000000000000000000011111111")]
    [InlineData(56, "1111111100000000000000000000000000000000000000000000000000000000")]
    [InlineData(63, "1111111100000000000000000000000000000000000000000000000000000000")]
    public void GeneratesHorizontalAttacksCorrectly(int cell, string expected)
    {
        var attack = _attacks[Kind.Queen][Direction.Horizontal][cell];
        
        Assert.Equal(expected, Convert.ToString((long) attack, 2).PadLeft(Constants.Cells, '0'));
    }
    
    [Theory]
    [InlineData(0, "0000000100000001000000010000000100000001000000010000000100000001")]
    [InlineData(7, "1000000010000000100000001000000010000000100000001000000010000000")]
    [InlineData(56, "0000000100000001000000010000000100000001000000010000000100000001")]
    [InlineData(63, "1000000010000000100000001000000010000000100000001000000010000000")]
    public void GeneratesVerticalAttacksCorrectly(int cell, string expected)
    {
        var attack = _attacks[Kind.Queen][Direction.Vertical][cell];
        
        Assert.Equal(expected, Convert.ToString((long) attack, 2).PadLeft(Constants.Cells, '0'));
    }
}