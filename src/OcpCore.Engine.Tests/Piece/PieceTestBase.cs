using Xunit;

namespace OcpCore.Engine.Tests.Piece;

public class PieceTestBase
{
    protected static void AssertExpectedMoves(string expected, List<int> moves)
    {
        var expectedMoves = expected.Split(',').Select(int.Parse).ToArray();

        foreach (var move in expectedMoves)
        {
            Assert.Contains(move, moves);
        }

        foreach (var move in moves)
        {
            Assert.Contains(move, expectedMoves);
        }
        
        Assert.Equal(expectedMoves.Length, moves.Count);
    }
}