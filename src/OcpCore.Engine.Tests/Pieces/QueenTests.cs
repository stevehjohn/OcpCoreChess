using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class QueenTests : PieceTestBase<Queen>
{
    [Theory]
    [InlineData(0, "1,2,3,4,5,6,7,8,16,24,32,40,48,56,9,18,27,36,45,54,63")]
    [InlineData(35, "32,33,34,36,37,38,39,59,51,43,27,19,11,3,42,49,56,26,17,8,44,53,62,28,21,14,7")]
    public void MovesDirectionallyAsExpectedOnEmptyBoard(int position, string expectedMoves)
    {
        var board = new Board();

        var piece = Piece;

        var moves = new List<Move>();
        
        piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/3p4 b - - 0 1", 0, "1,2,8,16,24,32,40,48,56,9,18,27,36,45,54,63")]
    [InlineData("8/8/8/8/8/2p5/8/8 b - - 0 1", 0, "1,2,3,4,5,6,7,8,16,24,32,40,48,56,9")]
    [InlineData("8/p7/8/8/8/8/8/8 b - - 0 1", 0, "1,2,3,4,5,6,7,8,16,24,32,40,9,18,27,36,45,54,63")]
    [InlineData("8/8/8/1p5p/8/8/8/8 b - - 0 1", 35, "34,36,37,38,59,51,43,27,19,11,3,42,49,56,26,17,8,44,53,62,28,21,14,7")]
    [InlineData("8/3p4/8/8/8/8/3p4/8 b - - 0 1", 35, "32,33,34,36,37,38,39,43,27,19,42,49,56,26,17,8,44,53,62,28,21,14,7")]
    [InlineData("8/1p6/8/8/8/8/8/7p b -- 0 1", 35, "32,33,34,36,37,38,39,59,51,43,27,19,11,3,42,26,17,8,44,53,62,28,21,14")]
    [InlineData("6p1/8/8/8/8/8/p7/8 b -- 0 1", 35, "32,33,34,36,37,38,39,59,51,43,27,19,11,3,42,49,56,26,17,44,53,28,21,14,7")]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        var piece = Piece;

        var moves = new List<Move>();
        
        piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/3P4 b - - 0 1", 0, "1,2,3,8,16,24,32,40,48,56,9,18,27,36,45,54,63")]
    [InlineData("8/8/8/8/8/2P5/8/8 b - - 0 1", 0, "1,2,3,4,5,6,7,8,16,24,32,40,48,56,9,18")]
    [InlineData("8/P7/8/8/8/8/8/8 b - - 0 1", 0, "1,2,3,4,5,6,7,8,16,24,32,40,48,9,18,27,36,45,54,63")]
    [InlineData("8/8/8/1p5p/8/8/8/8 b - - 0 1", 35, "34,36,37,38,59,51,43,27,19,11,3,42,49,56,26,17,8,44,53,62,28,21,14,7")]
    [InlineData("8/3p4/8/8/8/8/3p4/8 b - - 0 1", 35, "32,33,34,36,37,38,39,43,27,19,42,49,56,26,17,8,44,53,62,28,21,14,7")]
    [InlineData("8/1P6/8/8/8/8/8/7P b -- 0 1", 35, "32,33,34,36,37,38,39,59,51,43,27,19,11,3,42,49,26,17,8,44,53,62,28,21,14,7")]
    [InlineData("6P1/8/8/8/8/8/P7/8 b -- 0 1", 35, "32,33,34,36,37,38,39,59,51,43,27,19,11,3,42,49,56,26,17,8,44,53,62,28,21,14,7")]
    public void TakesPieceOfOpposingColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        var piece = Piece;

        var moves = new List<Move>();
        
        piece.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }

    [Theory]
    [InlineData("8/8/8/P7/8/8/8/8 b - - 0 1", 0, true, 32)]
    [InlineData("8/8/8/p7/8/8/8/8 b - - 0 1", 0, false, 0)]
    public void ReportsCapturesCorrectly(string fen, int position, bool captureExpected, int captureCell)
    {
        var board = new Board(fen);

        var piece = Piece;

        var moves = new List<Move>();
        
        piece.GetMoves(board, position, Colour.Black, moves);

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