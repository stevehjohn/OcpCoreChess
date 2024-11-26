using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class KingTests : PieceTestBase<King>
{
    [Theory]
    [InlineData("8/8/8/8/8/8/8/k7 b - - 0 1", 0, "8,9,1")]
    [InlineData("8/8/8/3k4/8/8/8/8 b - - 0 1", 35, "42,43,44,34,36,26,27,28")]
    public void MovesDirectionallyAsExpectedOnEmptyBoard(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        AssertPieceIsWhereExpected(board, position, Colour.Black);

        var moves = new List<Move>();
        
        Piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/p7/k7 b - - 0 1", 0, "9,1")]
    [InlineData("8/8/8/8/8/8/1p6/k7 b - - 0 1", 0, "8,1")]
    [InlineData("8/8/8/8/8/8/8/kp6 b - - 0 1", 0, "8,9")]
    [InlineData("8/8/2pp4/3k4/8/8/8/8 b - - 0 1", 35, "44,34,36,26,27,28")]
    [InlineData("8/8/8/3kp3/4p3/8/8/8 b - - 0 1", 35, "42,43,44,34,26,27")]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        AssertPieceIsWhereExpected(board, position, Colour.Black);

        var moves = new List<Move>();
        
        Piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/P7/k7 b - - 0 1", 0, "8,9,1")]
    [InlineData("8/8/8/8/8/8/1P6/k7 b - - 0 1", 0, "8,9,1")]
    [InlineData("8/8/8/8/8/8/8/kP6 b - - 0 1", 0, "8,9,1")]
    [InlineData("8/8/2PP4/3k4/8/8/8/8 b - - 0 1", 35, "42,43,44,34,36,26,27,28")]
    [InlineData("8/8/8/3kP3/4P3/8/8/8 b - - 0 1", 35, "42,43,44,34,36,26,27,28")]
    public void TakesPieceOfOpposingColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        AssertPieceIsWhereExpected(board, position, Colour.Black);

        var moves = new List<Move>();
        
        Piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }

    [Theory]
    [InlineData("8/8/8/8/8/8/1P6/k7 b - - 0 1", 0, true, 9)]
    [InlineData("8/8/8/8/8/8/1p6/k7 b - - 0 1", 0, false, 0)]
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