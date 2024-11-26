using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class KnightTests : PieceTestBase<Knight>
{
    [Theory]
    [InlineData("8/8/8/8/8/8/8/n7 b - - 0 1", 0, "17,10")]
    [InlineData("8/8/8/3n4/8/8/8/8 b - - 0 1", 35, "41,25,45,29,50,52,18,20")]
    public void MovesAccordingToKnightRulesOnEmptyBoard(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        AssertPieceIsWhereExpected(board, position, Colour.Black);

        var moves = new List<Move>();
        
        Piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/1p6/8/n7 b - - 0 1", 0, "10")]
    [InlineData("8/8/8/8/8/8/2p5/n7 b - - 0 1", 0, "17")]
    [InlineData("8/8/1p6/3n4/8/4p3/8/8 b - - 0 1", 35, "25,45,29,50,52,18")]
    [InlineData("8/8/5p2/3n4/5p2/8/8/8 b - - 0 1", 35, "41,25,50,52,18,20")]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        AssertPieceIsWhereExpected(board, position, Colour.Black);

        var moves = new List<Move>();
        
        Piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/1P6/8/n7 b - - 0 1", 0, "17,10")]
    [InlineData("8/8/8/8/8/8/2P5/n7 b - - 0 1", 0, "17,10")]
    [InlineData("8/8/1P6/3n4/8/4P3/8/8 b - - 0 1", 35, "41,25,45,29,50,52,18,20")]
    [InlineData("8/8/5P2/3n4/5P2/8/8/8 b - - 0 1", 35, "41,25,45,29,50,52,18,20")]
    public void TakesPieceOfOpposingColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        AssertPieceIsWhereExpected(board, position, Colour.Black);

        var moves = new List<Move>();
        
        Piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }

    [Theory]
    [InlineData("8/8/8/8/8/1P6/8/n7 b - - 0 1", 0, true, 17)]
    [InlineData("8/8/8/8/8/1p6/8/n7 b - - 0 1", 0, false, 0)]
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