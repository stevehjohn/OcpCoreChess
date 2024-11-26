using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.General;

public class StateTests
{
    [Theory]
    [InlineData(Colour.White, 49, Castle.WhiteQueenSide | Castle.BlackKingSide)]
    [InlineData(Colour.Black, 42, Castle.WhiteQueenSide | Castle.BlackKingSide)]
    [InlineData(Colour.White, 18, Castle.NotAvailable)]
    [InlineData(Colour.Black, 34, Castle.NotAvailable)]
    [InlineData(Colour.White, 30, Castle.WhiteQueenSide)]
    [InlineData(Colour.Black, 50, Castle.BlackKingSide)]
    [InlineData(Colour.White, 33, Castle.WhiteQueenSide | Castle.WhiteKingSide | Castle.BlackQueenSide | Castle.BlackKingSide)]
    [InlineData(Colour.Black, 29, Castle.WhiteQueenSide | Castle.WhiteKingSide | Castle.BlackQueenSide | Castle.BlackKingSide)]
    public void PacksAndUnpacksStateCorrectly(Colour player, int enPassantTarget, Castle castleStatus)
    {
        var state = new State(player, enPassantTarget, castleStatus);
        
        Assert.Equal(player, state.Player);
        
        Assert.Equal(enPassantTarget, state.EnPassantTarget);
        
        Assert.Equal(castleStatus, state.CastleStatus);
    }
}