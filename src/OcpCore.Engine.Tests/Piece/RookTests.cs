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
}