using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Piece;

public class BishopTests : PieceTestBase
{
    [Theory]
    [InlineData(0, "9,18,27,36,45,54,63")]
    [InlineData(35, "42,49,56,26,17,8,44,53,62,28,21,14,7")]
    public void MovesDiagonallyAsExpectedOnEmptyBoard(int position, string expectedMoves)
    {
        var board = new Board();

        var bishop = new Bishop();

        var moves = new List<int>();
        
        bishop.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/3p4/8/8/8 b - - 0 1", 0, "9,18")]
    [InlineData("p7/8/8/8/8/1p6/8/8 b - - 0 1", 35, "42,49,26,44,53,62,28,21,14,7")]
    [InlineData("8/1p6/8/8/8/8/8/7p", 35, "42,26,17,8,44,53,62,28,21,14")]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        var bishop = new Bishop();

        var moves = new List<int>();
        
        bishop.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/3P4/8/8/8 b - - 0 1", 0, "9,18,27")]
    [InlineData("P7/8/8/8/8/1P6/8/8 b - - 0 1", 35, "42,49,56,26,17,44,53,62,28,21,14,7")]
    [InlineData("8/1P6/8/8/8/8/8/7P", 35, "42,49,26,17,8,44,53,62,28,21,14,7")]
    public void TakesPieceOfOpposingColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        var bishop = new Bishop();

        var moves = new List<int>();
        
        bishop.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
}