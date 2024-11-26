using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;
using Xunit.Sdk;

namespace OcpCore.Engine.Tests.Pieces;

public class PieceTestBaseTests : PieceTestBase<Pawn>
{
    [Theory]
    [InlineData("8/8/8/8/8/8/8/p7 b - - 0 1", 0, Colour.Black, false)]
    [InlineData("8/8/8/8/8/8/8/P7 b - - 0 1", 0, Colour.Black, true)]
    [InlineData("8/8/8/8/8/8/8/k7 b - - 0 1", 0, Colour.Black, true)]
    [InlineData("8/8/8/8/8/8/8/K7 b - - 0 1", 0, Colour.Black, true)]
    public void SelfTestAssertPieceIsWhereExpected(string fen, int position, Colour colour, bool shouldThrow)
    {
        var board = new Board(fen);

        if (! shouldThrow)
        {
            AssertPieceIsWhereExpected(board, position, colour);
        }
        else
        {
            Assert.Throws<TrueException>(() => AssertPieceIsWhereExpected(board, position, colour));
        }
    }
}