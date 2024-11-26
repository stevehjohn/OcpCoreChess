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
}