using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Piece;

public class QueenTests : PieceTestBase
{
    [Theory]
    [InlineData(0, "1,2,3,4,5,6,7,8,16,24,32,40,48,56,9,18,27,36,45,54,63")]
    [InlineData(35, "32,33,34,36,37,38,39,59,51,43,27,19,11,3,42,49,56,26,17,8,44,53,62,28,21,14,7")]
    public void MovesDirectionallyAsExpectedOnEmptyBoard(int position, string expectedMoves)
    {
        var board = new Board();

        var queen = new Queen();

        var moves = new List<int>();
        
        queen.GetMoves(board, position, Colour.Black, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
}