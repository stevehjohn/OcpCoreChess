using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class PieceTestBase<T> where T : Piece, new()
{
    protected Piece Piece => new T();
    
    protected static void AssertExpectedMoves(string expected, List<Move> moves)
    {
        if (expected == null)
        {
            Assert.Empty(moves);
            
            return;
        }

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