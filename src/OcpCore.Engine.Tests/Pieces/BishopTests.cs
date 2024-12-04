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
    
    // [Theory]
    // [InlineData("8/8/8/8/3p4/8/8/b7 b - - 0 1", 0, "9,18")]
    // [InlineData("p7/8/8/3b4/8/1p6/8/8 b - - 0 1", 35, "42,49,26,44,53,62,28,21,14,7")]
    // [InlineData("8/1p6/8/3b4/8/8/8/7p b - - 0 1", 35, "42,26,17,8,44,53,62,28,21,14")]
    // public void IsBlockedByPieceOfOwnColour(string fen, int position, string expectedMoves)
    // {
    //     var board = new Board(fen);
    //
    //     AssertPieceIsWhereExpected(board, position, Colour.Black);
    //
    //     var moves = new List<Move>();
    //     
    //     Piece.GetMoves(board, position, Colour.Black, moves);
    //     
    //     AssertExpectedMoves(expectedMoves, moves);
    // }
    //
    // [Theory]
    // [InlineData("8/8/8/8/3P4/8/8/b7 b - - 0 1", 0, "9,18,27")]
    // [InlineData("P7/8/8/3b4/8/1P6/8/8 b - - 0 1", 35, "42,49,56,26,17,44,53,62,28,21,14,7")]
    // [InlineData("8/1P6/8/3b4/8/8/8/7P b - - 0 1", 35, "42,49,26,17,8,44,53,62,28,21,14,7")]
    // public void TakesPieceOfOpposingColour(string fen, int position, string expectedMoves)
    // {
    //     var board = new Board(fen);
    //
    //     AssertPieceIsWhereExpected(board, position, Colour.Black);
    //
    //     var moves = new List<Move>();
    //     
    //     Piece.GetMoves(board, position, Colour.Black, moves);
    //     
    //     AssertExpectedMoves(expectedMoves, moves);
    // }
    //
    // [Theory]
    // [InlineData("8/8/8/8/8/2P5/8/b7 b - - 0 1", 0, true, 18)]
    // [InlineData("8/8/8/8/8/2p5/8/b7 b - - 0 1", 0, false, 0)]
    // public void ReportsCapturesCorrectly(string fen, int position, bool captureExpected, int captureCell)
    // {
    //     var board = new Board(fen);
    //
    //     AssertPieceIsWhereExpected(board, position, Colour.Black);
    //
    //     var moves = new List<Move>();
    //     
    //     Piece.GetMoves(board, position, Colour.Black, moves);
    //
    //     if (captureExpected)
    //     {
    //         Assert.Single(moves, m => m.Outcome == MoveOutcome.Capture && m.Target == captureCell);
    //     }
    //     else
    //     {
    //         Assert.DoesNotContain(moves, m => m.Outcome == MoveOutcome.Capture);
    //     }
    // }
}