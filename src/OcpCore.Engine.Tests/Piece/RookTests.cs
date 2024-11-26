using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Piece;

public class RookTests : PieceTestBase
{
    [Theory]
    [InlineData(0, "1,2,3,4,5,6,7,8,16,24,32,40,48,56")]
    [InlineData(35, "32,33,34,36,37,38,39,59,51,43,27,19,11,3")]
    public void MovesOrthogonallyAsExpectedOnEmptyBoard(int position, string expectedMoves)
    {
        var board = new Board();

        var rook = new Rook();

        var moves = new List<int>();
        
        rook.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }

    [Theory]
    [InlineData("8/8/8/p7/8/8/8/8 b - - 0 1", 0, "1,2,3,4,5,6,7,8,16,24")]
    [InlineData("8/8/8/8/8/8/8/3p4 b - - 0 1", 0, "1,2,8,16,24,32,40,48,56")]
    [InlineData("8/8/8/1p5p/8/8/8/8 b - - 0 1", 35, "34,36,37,38,59,51,43,27,19,11,3")]
    [InlineData("3p4/8/8/8/8/8/3p4/8 b - - 0 1", 35, "32,33,34,36,37,38,39,51,43,27,19")]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, string expectedMoves)
    {
        var board = new Board(fen);

        var rook = new Rook();

        var moves = new List<int>();
        
        rook.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
}