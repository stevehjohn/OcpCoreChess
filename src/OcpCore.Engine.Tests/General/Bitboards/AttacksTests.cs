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
    [InlineData(0, "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "11111111")]
    [InlineData(7, "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "11111111")]
    [InlineData(56, "11111111" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000")]
    [InlineData(63, "11111111" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000")]
    public void GeneratesHorizontalAttacksCorrectly(int cell, string expected)
    {
        var attack = _attacks[Kind.Queen][Direction.Horizontal][cell];
        
        Assert.Equal(expected, Convert.ToString((long) attack, 2).PadLeft(Constants.Cells, '0'));
    }
    
    [Theory]
    [InlineData(0, "00000001" +
                   "00000001" +
                   "00000001" +
                   "00000001" +
                   "00000001" +
                   "00000001" +
                   "00000001" +
                   "00000001")]
    [InlineData(7, "10000000" +
                   "10000000" +
                   "10000000" +
                   "10000000" +
                   "10000000" +
                   "10000000" +
                   "10000000" +
                   "10000000")]
    [InlineData(56, "00000001" +
                    "00000001" +
                    "00000001" +
                    "00000001" +
                    "00000001" +
                    "00000001" +
                    "00000001" +
                    "00000001")]
    [InlineData(63, "10000000" +
                    "10000000" +
                    "10000000" +
                    "10000000" +
                    "10000000" +
                    "10000000" +
                    "10000000" +
                    "10000000")]
    public void GeneratesVerticalAttacksCorrectly(int cell, string expected)
    {
        var attack = _attacks[Kind.Queen][Direction.Vertical][cell];
        
        Assert.Equal(expected, Convert.ToString((long) attack, 2).PadLeft(Constants.Cells, '0'));
    }
}