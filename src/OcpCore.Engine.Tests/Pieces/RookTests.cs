using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class RookTests
{
    private readonly Rook _rook = new();
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/R7 w - - 0 1", 0,
        0b0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_1111_1110)]
    [InlineData("8/8/8/3r4/8/8/8/8 b - - 0 1", 35,
        0b0000_1000_0000_1000_0000_1000_1111_0111_0000_1000_0000_1000_0000_1000_0000_1000)]
    public void MovesOrthogonallyAsExpectedOnEmptyBoard(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Rook, position));

        var moves = _rook.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }

    [Theory]
    [InlineData("8/8/8/8/8/8/8/R4P1P w - - 0 1", 0,
        0b0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0001_1110)]
    [InlineData("8/8/8/8/8/8/8/P1P4R w - - 0 1", 7,
        0b1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_0111_1000)]
    [InlineData("8/8/8/8/8/8/8/R2P4 w - - 0 1", 0,
        0b0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0110)]
    [InlineData("8/8/8/1P1R3P/8/8/8/8 w - - 0 1", 35,
        0b0000_1000_0000_1000_0000_1000_0111_0100_0000_1000_0000_1000_0000_1000_0000_1000)]
    [InlineData("3P4/8/8/3R4/8/8/3P4/8 w - - 0 1", 35,
        0b0000_0000_0000_1000_0000_1000_1111_0111_0000_1000_0000_1000_0000_0000_0000_0000)]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Rook, position));

        var moves = _rook.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/R4p1P w - - 0 1", 0,
        0b0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0011_1110)]
    [InlineData("8/8/8/8/8/8/8/P2p3R w - - 0 1", 7,
        0b1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_0111_1000)]
    [InlineData("8/8/8/1p1R3p/8/8/8/8 w - - 0 1", 35, 
        0b0000_1000_0000_1000_0000_1000_1111_0110_0000_1000_0000_1000_0000_1000_0000_1000)]
    [InlineData("3P4/8/8/3R4/8/8/3p4/8 w - - 0 1", 35,
        0b0000_0000_0000_1000_0000_1000_1111_0111_0000_1000_0000_1000_0000_1000_0000_0000)]
    public void TakesPieceOfOpposingColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);
    
        Assert.True(game.IsKind(Kind.Rook, position));
    
        var moves = _rook.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
}