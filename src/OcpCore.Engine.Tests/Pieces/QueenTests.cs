using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class QueenTests
{
    private readonly Queen _queen = new();
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/Q7 w - - 0 1", 0,
        0b1000_0001_0100_0001_0010_0001_0001_0001_0000_1001_0000_0101_0000_0011_1111_1110)]
    [InlineData("8/8/8/8/8/8/8/7Q w - - 0 1", 7,
        0b1000_0001_1000_0010_1000_0100_1000_1000_1001_0000_1010_0000_1100_0000_0111_1111)]
    [InlineData("8/8/8/3Q4/8/8/8/8 w - - 0 1", 35,
        0b0100_1001_0010_1010_0001_1100_1111_0111_0001_1100_0010_1010_0100_1001_1000_1000)]
    public void MovesDirectionallyAsExpectedOnEmptyBoard(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Queen, position));

        var moves = _queen.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("7q/6pp/8/8/4P3/8/8/8 b - - 0 1", 63, 
        0b0111_1111_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Queen, position));

        var moves = _queen.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("7q/6pP/8/8/4P3/8/8/8 b - - 0 1", 63, 
        0b0111_1111_1000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    public void TakesPieceOfOpposingColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Queen, position));

        var moves = _queen.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
}