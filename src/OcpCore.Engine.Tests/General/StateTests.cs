using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.General;

public class StateTests
{
    [Theory]
    [InlineData(Colour.White, Castle.WhiteQueenSide | Castle.BlackKingSide, 49, 15, 30)]
    [InlineData(Colour.Black, Castle.WhiteQueenSide | Castle.BlackKingSide, 42, 25, 10)]
    [InlineData(Colour.White, Castle.NotAvailable, 18, 40, 20)]
    [InlineData(Colour.Black, Castle.NotAvailable, 34, 20, 40)]
    [InlineData(Colour.White, Castle.WhiteQueenSide, 30, 12, 34)]
    [InlineData(Colour.Black, Castle.BlackKingSide, 50, 43, 21)]
    [InlineData(Colour.White, Castle.WhiteQueenSide | Castle.WhiteKingSide | Castle.BlackQueenSide | Castle.BlackKingSide, 33, 98, 76)]
    [InlineData(Colour.Black, Castle.WhiteQueenSide | Castle.WhiteKingSide | Castle.BlackQueenSide | Castle.BlackKingSide, 29, 67, 89)]
    [InlineData(Colour.White, Castle.NotAvailable, null, 12, 13)]
    [InlineData(Colour.Black, Castle.NotAvailable, null, 14, 15)]
    public void PacksAndUnpacksStateCorrectly(Colour player, Castle castleStatus, int? enPassantTarget, int whiteScore, int blackScore)
    {
        var state = new State(player, castleStatus, enPassantTarget, whiteScore, blackScore);
        
        Assert.Equal(player, state.Player);
        
        Assert.Equal(enPassantTarget, state.EnPassantTarget);
        
        Assert.Equal(castleStatus, state.CastleStatus);
        
        Assert.Equal(whiteScore, state.WhiteScore);
        
        Assert.Equal(blackScore, state.BlackScore);
    }
    
    [Theory]
    [InlineData(Colour.White, Castle.WhiteQueenSide | Castle.BlackKingSide, 49, 12, 34, Castle.WhiteQueenSide, Castle.BlackKingSide)]
    [InlineData(Colour.White, Castle.WhiteQueenSide | Castle.BlackKingSide, 49, 56, 78, Castle.WhiteKingSide, Castle.WhiteQueenSide | Castle.BlackKingSide)]
    [InlineData(Colour.Black, Castle.WhiteQueenSide | Castle.BlackKingSide, 42, 12, 34, Castle.BlackKingSide, Castle.WhiteQueenSide)]
    [InlineData(Colour.White, Castle.NotAvailable, 18, 56, 78, Castle.WhiteKingSide, Castle.NotAvailable)]
    [InlineData(Colour.Black, Castle.NotAvailable, 34, 12, 34, Castle.BlackQueenSide, Castle.NotAvailable)]
    [InlineData(Colour.White, Castle.WhiteQueenSide, 30, 56, 78, Castle.WhiteQueenSide, Castle.NotAvailable)]
    [InlineData(Colour.Black, Castle.BlackKingSide, 50, 0, 0, Castle.BlackKingSide, Castle.NotAvailable)]
    [InlineData(Colour.White, Castle.NotAvailable, null, 56, 78, Castle.WhiteKingSide, Castle.NotAvailable)]
    [InlineData(Colour.Black, Castle.NotAvailable, null, 0, 0, Castle.BlackQueenSide, Castle.NotAvailable)]
    public void RemoveCastleRightsOperatesCorrectly(Colour player, Castle castleStatus, int? enPassantTarget, int whiteScore, int blackScore, Castle rightToRemove, Castle expectedRights)
    {
        var state = new State(player, castleStatus, enPassantTarget, whiteScore, blackScore);
        
        Assert.Equal(player, state.Player);
        
        Assert.Equal(enPassantTarget, state.EnPassantTarget);
        
        Assert.Equal(castleStatus, state.CastleStatus);
        
        state.RemoveCastleRights(rightToRemove);
        
        Assert.Equal(player, state.Player);
        
        Assert.Equal(enPassantTarget, state.EnPassantTarget);
        
        Assert.Equal(expectedRights, state.CastleStatus);
        
        Assert.Equal(whiteScore, state.WhiteScore);
        
        Assert.Equal(blackScore, state.BlackScore);
    }
    
    // [Theory]
    // [InlineData(Colour.White, Castle.WhiteQueenSide | Castle.BlackKingSide, 49, 32)]
    // [InlineData(Colour.Black, Castle.WhiteQueenSide | Castle.BlackKingSide, 42, null)]
    // [InlineData(Colour.White, Castle.NotAvailable, 18, 55)]
    // [InlineData(Colour.Black, Castle.NotAvailable, 34, null)]
    // [InlineData(Colour.White, Castle.WhiteQueenSide, 30, 50)]
    // [InlineData(Colour.Black, Castle.BlackKingSide, 50, null)]
    // [InlineData(Colour.White, Castle.WhiteQueenSide | Castle.WhiteKingSide | Castle.BlackQueenSide | Castle.BlackKingSide, 33, 54)]
    // [InlineData(Colour.Black, Castle.All, 29, null)]
    // [InlineData(Colour.White, Castle.NotAvailable, null, 12)]
    // [InlineData(Colour.Black, Castle.NotAvailable, null, null)]
    // public void SetEnPassantTargetOperatesCorrectly(Colour player, Castle castleStatus, int? enPassantTarget, int? newEnPassantTarget, int whiteScore, int blackScore)
    // {
    //     var state = new State(player, castleStatus, enPassantTarget, whiteScore, blackScore);
    //     
    //     Assert.Equal(player, state.Player);
    //     
    //     Assert.Equal(enPassantTarget, state.EnPassantTarget);
    //     
    //     Assert.Equal(castleStatus, state.CastleStatus);
    //     
    //     state.SetEnPassantTarget(newEnPassantTarget);
    //     
    //     Assert.Equal(player, state.Player);
    //     
    //     Assert.Equal(newEnPassantTarget, state.EnPassantTarget);
    //     
    //     Assert.Equal(castleStatus, state.CastleStatus);
    // }
}