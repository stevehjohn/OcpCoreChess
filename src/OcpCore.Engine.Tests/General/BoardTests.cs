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
    [InlineData("8/8/pp5/8/8/8/8/8 w - - 0 1", "Not enough files in rank 6: pp5.")]
    [InlineData("pppppppp/8/8/8/8/8/8 w - - 0 1", "Incorrect number of ranks in FEN string: 7.")]
    [InlineData("pppppppp/8/8/8/8/8/8/8/8 w - - 0 1", "Incorrect number of ranks in FEN string: 9.")]
    [InlineData("pppppppp/8/8/8/8/8/8/ppgppppp w - - 0 1", "Invalid piece token in rank 1: g.")]
    [InlineData("pppppppp/8/8/8/8/8/8/pppppppp - - 0 1", "Invalid number of parts to FEN string: 5.")]
    [InlineData("pppppppp/8/8/8/8/8/8/pppppppp x - - 0 1", "Invalid turn indicator: x.")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w XQkq - 0 1", "Invalid castling status indicator: X")]
    public void BoardDetectsInvalidFenSituations(string fen, string expectedMessage)
    {
        var exception = Assert.Throws<FenParseException>(() => new Board(fen));
        
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 8, 16, Castle.All, Castle.All)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/1PPPPPPP/RNBQKBNR w KQkq - 0 1", 0, 16, Castle.All, Castle.All ^ Castle.WhiteQueenSide)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPP1/RNBQKBNR w KQkq - 0 1", 7, 23, Castle.All, Castle.All ^ Castle.WhiteKingSide)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1", 4, 12, Castle.All, Castle.BlackQueenSide | Castle.BlackKingSide)]
    [InlineData("rnbqkbnr/1ppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 56, 40, Castle.All, Castle.All ^ Castle.BlackQueenSide)]
    [InlineData("rnbqkbnr/ppppppp1/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 63, 55, Castle.All, Castle.All ^ Castle.BlackKingSide)]
    [InlineData("rnbqkbnr/pppp1ppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 60, 52, Castle.All, Castle.WhiteQueenSide | Castle.WhiteKingSide)]
    public void BoardUpdatesCastlingRightsOnMove(string fen, int position, int target, Castle rightsBeforeMove, Castle rightsAfterMove)
    {
        var board = new Board(fen);
        
        Assert.Equal(rightsBeforeMove, board.State.CastleStatus);
        
        board.MakeMove(position, target);
        
        Assert.Equal(rightsAfterMove, board.State.CastleStatus);
    }
    
    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 8, 16, null)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 8, 24, 16)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 10, 26, 18)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 48, 40, null)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 48, 32, 40)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 50, 34, 42)]
    public void BoardUpdatesEnPassantCellOnMove(string fen, int position, int target, int? enPassantCell)
    {
        var board = new Board(fen);
        
        Assert.Null(board.State.EnPassantTarget);
        
        board.MakeMove(position, target);
        
        Assert.Equal(enPassantCell, board.State.EnPassantTarget);
    }

    [Theory]
    [InlineData("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 60, 58, "  kr bnr/pppppppp/        /        /        /        /PPPPPPPP/RNBQKBNR")]
    [InlineData("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 60, 62, "r    rk /pppppppp/        /        /        /        /PPPPPPPP/RNBQKBNR")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR b KQkq - 0 1", 4, 2, "rnbqkbnr/pppppppp/        /        /        /        /PPPPPPPP/  KR BNR")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1", 4, 6, "rnbqkbnr/pppppppp/        /        /        /        /PPPPPPPP/R    RK ")]
    public void BoardPerformsCastle(string fen, int position, int target, string expectedFem)
    {
        var board = new Board(fen);
        
        board.MakeMove(position, target);
        
        Assert.Equal(expectedFem, board.ToString());
    }

    [Theory]
    [InlineData("rnbqkbnr/pppp1ppp/8/4pP2/8/8/PPPPP1PP/RNBQKBNR w KQkq e6 0 1", 37, 44, "rnbqkbnr/pppp ppp/    P   /        /        /        /PPPPP PP/RNBQKBNR")]
    [InlineData("rnbqkbnr/pppp1ppp/8/8/4pP2/8/PPPPP1PP/RNBQKBNR b KQkq f3 0 1", 28, 21, "rnbqkbnr/pppp ppp/        /        /        /     p  /PPPPP PP/RNBQKBNR")]
    public void BoardPerformsEnPassant(string fen, int position, int target, string expectedFem)
    {
        var board = new Board(fen);
        
        board.MakeMove(position, target);
        
        Assert.Equal(expectedFem, board.ToString());
    }
}