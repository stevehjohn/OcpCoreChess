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
    
    [Theory]
    [InlineData(0, "10000000" +
                   "01000000" +
                   "00100000" +
                   "00010000" +
                   "00001000" +
                   "00000100" +
                   "00000010" +
                   "00000001")]
    [InlineData(1, "00000000" +
                   "10000000" +
                   "01000000" +
                   "00100000" +
                   "00010000" +
                   "00001000" +
                   "00000100" +
                   "00000010")]
    [InlineData(6, "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "10000000" +
                   "01000000")]
    [InlineData(7, "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "10000000")]
    [InlineData(8, "01000000" +
                   "00100000" +
                   "00010000" +
                   "00001000" +
                   "00000100" +
                   "00000010" +
                   "00000001" + 
                   "00000000")]
    [InlineData(48, "00000010" +
                    "00000001" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" + 
                    "00000000")]
    [InlineData(56, "00000001" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" + 
                    "00000000")]
    public void GeneratesDiagonalAttacksCorrectly(int cell, string expected)
    {
        var attack = _attacks[Kind.Queen][Direction.Diagonal][cell];
        
        Assert.Equal(expected, Convert.ToString((long) attack, 2).PadLeft(Constants.Cells, '0'));
    }

    [Theory]
    [InlineData(7, "00000001" +
                   "00000010" +
                   "00000100" +
                   "00001000" +
                   "00010000" +
                   "00100000" +
                   "01000000" +
                   "10000000")]
    [InlineData(6, "00000000" +
                   "00000001" +
                   "00000010" +
                   "00000100" +
                   "00001000" +
                   "00010000" +
                   "00100000" +
                   "01000000")]
    [InlineData(1, "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000001" +
                   "00000010")]
    [InlineData(0, "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000000" +
                   "00000001")]
    [InlineData(15, "00000010" +
                    "00000100" +
                    "00001000" +
                    "00010000" +
                    "00100000" +
                    "01000000" +
                    "10000000" +
                    "00000000")]
    [InlineData(55, "01000000" +
                    "10000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000")]
    [InlineData(62, "10000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000" +
                    "00000000")]
    public void GeneratesAntiDiagonalAttacksCorrectly(int cell, string expected)
    {
        var attack = _attacks[Kind.Queen][Direction.AntiDiagonal][cell];

        Assert.Equal(expected, Convert.ToString((long) attack, 2).PadLeft(Constants.Cells, '0'));
    }
}