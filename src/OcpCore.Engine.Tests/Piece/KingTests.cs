using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Piece;

public class KingTests : PieceTestBase
{
    [Theory]
    [InlineData(0, "8,9,1")]
    [InlineData(35, "42,43,44,34,36,26,27,28")]
    public void MovesDirectionallyAsExpectedOnEmptyBoard(int position, string expectedMoves)
    {
        var board = new Board();

        var king = new King();

        var moves = new List<Move>();
        
        king.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/p7/8 b - - 0 1", 0, "9,1")]
    [InlineData("8/8/8/8/8/8/1p6/8 b - - 0 1", 0, "8,1")]
    [InlineData("8/8/8/8/8/8/8/1p6 b - - 0 1", 0, "8,9")]
    [InlineData("8/8/2pp4/8/8/8/8/8 b - - 0 1", 35, "44,34,36,26,27,28")]
    [InlineData("8/8/8/4p3/4p3/8/8/8 b - - 0 1", 35, "42,43,44,34,26,27")]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);
    
        var king = new King();
    
        var moves = new List<Move>();
        
        king.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/P7/8 b - - 0 1", 0, "8,9,1")]
    [InlineData("8/8/8/8/8/8/1P6/8 b - - 0 1", 0, "8,9,1")]
    [InlineData("8/8/8/8/8/8/8/1P6 b - - 0 1", 0, "8,9,1")]
    [InlineData("8/8/2PP4/8/8/8/8/8 b - - 0 1", 35, "42,43,44,34,36,26,27,28")]
    [InlineData("8/8/8/4P3/4P3/8/8/8 b - - 0 1", 35, "42,43,44,34,36,26,27,28")]
    public void TakesPieceOfOpposingColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);
    
        var king = new King();
    
        var moves = new List<Move>();
        
        king.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
}