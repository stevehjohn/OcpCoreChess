using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class BishopTests : PieceTestBase<Bishop>
{
    [Theory]
    [InlineData("8/8/8/8/8/8/8/b7 b - - 0 1", 0, "9,18,27,36,45,54,63")]
    [InlineData("8/8/8/3b4/8/8/8/8 b - - 0 1", 35, "42,49,56,26,17,8,44,53,62,28,21,14,7")]
    public void MovesDiagonallyAsExpectedOnEmptyBoard(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        AssertPieceIsWhereExpected(board, position, Colour.Black);

        var moves = new List<Move>();
        
        Piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/3p4/8/8/b7 b - - 0 1", 0, "9,18")]
    [InlineData("p7/8/8/3b4/8/1p6/8/8 b - - 0 1", 35, "42,49,26,44,53,62,28,21,14,7")]
    [InlineData("8/1p6/8/3b4/8/8/8/7p", 35, "42,26,17,8,44,53,62,28,21,14")]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        AssertPieceIsWhereExpected(board, position, Colour.Black);

        var moves = new List<Move>();
        
        Piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/3P4/8/8/b7 b - - 0 1", 0, "9,18,27")]
    [InlineData("P7/8/8/3b4/8/1P6/8/8 b - - 0 1", 35, "42,49,56,26,17,44,53,62,28,21,14,7")]
    [InlineData("8/1P6/8/3b4/8/8/8/7P b - - 0 1", 35, "42,49,26,17,8,44,53,62,28,21,14,7")]
    public void TakesPieceOfOpposingColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        AssertPieceIsWhereExpected(board, position, Colour.Black);

        var moves = new List<Move>();
        
        Piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }

    [Theory]
    [InlineData("8/8/8/8/8/2P5/8/b7 b - - 0 1", 0, true, 18)]
    [InlineData("8/8/8/8/8/2p5/8/b7 b - - 0 1", 0, false, 0)]
    public void ReportsCapturesCorrectly(string fen, int position, bool captureExpected, int captureCell)
    {
        var board = new Board(fen);

        AssertPieceIsWhereExpected(board, position, Colour.Black);

        var moves = new List<Move>();
        
        Piece.GetMoves(board, position, Colour.Black, moves);

        if (captureExpected)
        {
            Assert.Single(moves, m => m.Captures && m.NewPosition == captureCell);
        }
        else
        {
            Assert.True(! moves.Any(m => m.Captures));
        }
    }
}