using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class KnightTests
{
    private readonly Knight _knight = new();
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/n7 b - - 0 1", 0,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010_0000_0100_0000_0000)]
    [InlineData("8/8/8/3n4/8/8/8/8 b - - 0 1", 35,
        0b0000_0000_0001_0100_0010_0010_0000_0000_0010_0010_0001_0100_0000_0000_0000_0000)]
    public void MovesAccordingToKnightRulesOnEmptyBoard(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Knight, position));

        var moves = _knight.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/2P5/N7 w - - 0 1", 0,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010_0000_0000_0000_0000)]
    [InlineData("8/8/1P6/3N4/8/8/8/8 w - - 0 1", 35,
        0b0000_0000_0001_0100_0010_0000_0000_0000_0010_0010_0001_0100_0000_0000_0000_0000)]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Knight, position));

        var moves = _knight.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/2p5/N7 w - - 0 1", 0,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010_0000_0100_0000_0000)]
    [InlineData("8/8/1p6/3N4/8/8/8/8 w - - 0 1", 35,
        0b0000_0000_0001_0100_0010_0010_0000_0000_0010_0010_0001_0100_0000_0000_0000_0000)]
    public void TakesPieceOfOpposingColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Knight, position));

        var moves = _knight.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
}