using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class BishopTests
{
    private readonly Bishop _bishop = new();
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/B7 w - - 0 1", 0,
        0b1000_0000_0100_0000_0010_0000_0001_0000_0000_1000_0000_0100_0000_0010_0000_0000)]
    [InlineData("8/8/8/8/8/8/8/7B w - - 0 1", 7,
        0b0000_0001_0000_0010_0000_0100_0000_1000_0001_0000_0010_0000_0100_0000_0000_0000)]
    [InlineData("8/8/8/3B4/8/8/8/8 w - - 0 1", 35,
        0b0100_0001_0010_0010_0001_0100_0000_0000_0001_0100_0010_0010_0100_0001_1000_0000)]
    public void MovesDirectionallyAsExpectedOnEmptyBoard(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Bishop, position));

        var moves = _bishop.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/1P6/B7 w - - 0 1", 0,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    [InlineData("8/8/8/8/8/8/6P1/7B w - - 0 1", 7,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    [InlineData("8/8/2P5/3B4/8/8/8/8 w - - 0 1", 35,
        0b0100_0000_0010_0000_0001_0000_0000_0000_0001_0100_0010_0010_0100_0001_1000_0000)]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Bishop, position));

        var moves = _bishop.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/1p6/B7 w - - 0 1", 0,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010_0000_0000)]
    [InlineData("8/8/8/8/8/8/6p1/7B w - - 0 1", 7,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0100_0000_0000_0000)]
    [InlineData("8/8/2p5/3B4/8/8/8/8 w - - 0 1", 35,
        0b0100_0000_0010_0000_0001_0100_0000_0000_0001_0100_0010_0010_0100_0001_1000_0000)]
    public void TakesPieceOfOpposingColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Bishop, position));

        var moves = _bishop.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
}