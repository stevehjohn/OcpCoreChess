using Xunit;

namespace OcpCore.Engine.Tests.Piece;

public class PieceTestBase
{
    protected static void AssertExpectedMoves(string expected, List<int> moves)
    {
        var expectedMoves = expected.Split(',').Select(int.Parse).ToArray();
        
        Assert.Equal(expectedMoves.Length, moves.Count);

        foreach (var move in expectedMoves)
        {
            Assert.Contains(move, moves);
        }
    }
}