using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.General;

public class StateTests
{
    [Theory]
    [InlineData(Colour.White, Castle.WhiteQueenSide | Castle.BlackKingSide, 49)]
    [InlineData(Colour.Black, Castle.WhiteQueenSide | Castle.BlackKingSide, 42)]
    [InlineData(Colour.White, Castle.NotAvailable, 18)]
    [InlineData(Colour.Black, Castle.NotAvailable, 34)]
    [InlineData(Colour.White, Castle.WhiteQueenSide, 30)]
    [InlineData(Colour.Black, Castle.BlackKingSide, 50)]
    [InlineData(Colour.White, Castle.WhiteQueenSide | Castle.WhiteKingSide | Castle.BlackQueenSide | Castle.BlackKingSide, 33)]
    [InlineData(Colour.Black, Castle.WhiteQueenSide | Castle.WhiteKingSide | Castle.BlackQueenSide | Castle.BlackKingSide, 29)]
    [InlineData(Colour.White, Castle.NotAvailable, null)]
    [InlineData(Colour.Black, Castle.NotAvailable, null)]
    public void PacksAndUnpacksStateCorrectly(Colour player, Castle castleStatus, int? enPassantTarget)
    {
        var state = new State(player, castleStatus, enPassantTarget);
        
        Assert.Equal(player, state.Player);
        
        Assert.Equal(enPassantTarget, state.EnPassantTarget);
        
        Assert.Equal(castleStatus, state.CastleStatus);
    }
}