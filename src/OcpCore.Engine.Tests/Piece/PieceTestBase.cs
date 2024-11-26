using OcpCore.Engine.General;
using Xunit;

namespace OcpCore.Engine.Tests.Piece;

public class PieceTestBase<T> where T : Pieces.Piece, new()
{
    protected Pieces.Piece Piece => new T();
    
    protected static void AssertExpectedMoves(string expected, List<Move> moves)
    {
        var expectedMoves = expected.Split(',').Select(int.Parse).ToArray();

        foreach (var move in expectedMoves)
        {
            Assert.Contains(move, moves.Select(m => m.NewPosition));
        }

        foreach (var move in moves)
        {
            Assert.Contains(move.NewPosition, expectedMoves);
        }
        
        Assert.Equal(expectedMoves.Length, moves.Count);
    }
}