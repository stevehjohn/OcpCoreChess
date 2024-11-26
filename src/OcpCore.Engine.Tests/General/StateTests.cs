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
    
    [Theory]
    [InlineData(Colour.White, Castle.WhiteQueenSide | Castle.BlackKingSide, 49, Castle.WhiteQueenSide, Castle.BlackKingSide)]
    [InlineData(Colour.White, Castle.WhiteQueenSide | Castle.BlackKingSide, 49, Castle.WhiteKingSide, Castle.WhiteQueenSide | Castle.BlackKingSide)]
    [InlineData(Colour.Black, Castle.WhiteQueenSide | Castle.BlackKingSide, 42, Castle.BlackKingSide, Castle.WhiteQueenSide)]
    [InlineData(Colour.White, Castle.NotAvailable, 18, Castle.WhiteKingSide, Castle.NotAvailable)]
    [InlineData(Colour.Black, Castle.NotAvailable, 34, Castle.BlackQueenSide, Castle.NotAvailable)]
    [InlineData(Colour.White, Castle.WhiteQueenSide, 30, Castle.WhiteQueenSide, Castle.NotAvailable)]
    [InlineData(Colour.Black, Castle.BlackKingSide, 50, Castle.BlackKingSide, Castle.NotAvailable)]
    [InlineData(Colour.White, Castle.NotAvailable, null, Castle.WhiteKingSide, Castle.NotAvailable)]
    [InlineData(Colour.Black, Castle.NotAvailable, null, Castle.BlackQueenSide, Castle.NotAvailable)]
    public void RemoveCastleRightsOperatesCorrectly(Colour player, Castle castleStatus, int? enPassantTarget, Castle rightToRemove, Castle expectedRights)
    {
        var state = new State(player, castleStatus, enPassantTarget);
        
        Assert.Equal(player, state.Player);
        
        Assert.Equal(enPassantTarget, state.EnPassantTarget);
        
        Assert.Equal(castleStatus, state.CastleStatus);
        
        state.RemoveCastleRights(rightToRemove);
        
        Assert.Equal(player, state.Player);
        
        Assert.Equal(enPassantTarget, state.EnPassantTarget);
        
        Assert.Equal(expectedRights, state.CastleStatus);
    }
}