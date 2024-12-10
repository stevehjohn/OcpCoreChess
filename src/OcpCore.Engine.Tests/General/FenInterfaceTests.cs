using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Exceptions;
using OcpCore.Engine.General;
using Xunit;

namespace OcpCore.Engine.Tests.General;

public class FenInterfaceTests
{
    private Planes _planes;

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", null)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0", "Invalid number of parts")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "Invalid number of ranks")]
    [InlineData("rnbqkbnr/pppppppp/9/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "Too many files")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPPP/RNBQKBNR w KQkq - 0 1", "Too many files")]
    [InlineData("rnbqkbnr/pppXpppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "Invalid piece token")]
    [InlineData("rnbqkbnr/ppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "Not enough files")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR X KQkq - 0 1", "Invalid turn indicator")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KXkq - 0 1", "Invalid castling status")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - x 1", "Invalid value for halfmove")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 x", "Invalid value for fullmove")]
    public void ThrowsExpectedException(string fen, string expectedMessage)
    {
        if (expectedMessage == null)
        {
            FenInterface.ParseFen(fen, ref _planes);
        }
        else
        {
            var exception = Assert.Throws<FenParseException>(() => FenInterface.ParseFen(fen, ref _planes));

            Assert.Contains(expectedMessage, exception.Message);
        }
    }
}