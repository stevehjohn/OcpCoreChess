using OcpCore.Engine.Exceptions;
using OcpCore.Engine.General;
using Xunit;

namespace OcpCore.Engine.Tests.General;

public class BoardTests
{
    [Theory]
    [InlineData("8/8/8/8/8/8/8/9 b - - 0 1", "Too many files in rank 1: 9.")]
    [InlineData("ppppppppp/8/8/8/8/8/8/8 b - - 0 1", "Too many files in rank 8: ppppppppp.")]
    [InlineData("8/8/p7p/8/8/8/8/8 w - - 0 1", "Too many files in rank 6: p7p.")]
    [InlineData("8/8/pp7/8/8/8/8/8 w - - 0 1", "Too many files in rank 6: pp7.")]
    [InlineData("pppppppp/8/8/8/8/8/8 w - - 0 1", "Incorrect number of ranks in FEN string: 7.")]
    [InlineData("pppppppp/8/8/8/8/8/8/8/8 w - - 0 1", "Incorrect number of ranks in FEN string: 9.")]
    [InlineData("pppppppp/8/8/8/8/8/8/ppgppppp w - - 0 1", "Invalid piece token in rank 1: g.")]
    public void BoardDetectsInvalidFenSituations(string fen, string expectedMessage)
    {
        var exception =Assert.Throws<FenParseException>(() => new Board(fen));
        
        Assert.Equal(expectedMessage, exception.Message);
    }
}