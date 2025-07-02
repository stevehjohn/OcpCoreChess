using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using Xunit;

namespace OcpCore.Engine.Tests.Bitboards;

public class GameTests
{
    private Game _game = new();

    [Theory]
    [InlineData(
        0b00000000_00000000_00000000_00000000_00000000_00000000_11111111_11111111,
        0b11111111_11111111_00000000_00000000_00000000_00000000_00000000_00000000,
        0b00000000_11111111_00000000_00000000_00000000_00000000_11111111_00000000,
        0b10000001_00000000_00000000_00000000_00000000_00000000_00000000_10000001,
        0b01000010_00000000_00000000_00000000_00000000_00000000_00000000_01000010,
        0b00100100_00000000_00000000_00000000_00000000_00000000_00000000_00100100,
        0b00001000_00000000_00000000_00000000_00000000_00000000_00000000_00001000,
        0b00010000_00000000_00000000_00000000_00000000_00000000_00000000_00010000)]
    public void ParsesInitialFenCorrectly(ulong whitePlane, ulong blackPlane, ulong pawnPlane, ulong rookPlane, ulong knightPlane, ulong bishopPlane, ulong queenPlane, ulong kingPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);

        Assert.Equal(whitePlane, _game[Colour.White]);

        Assert.Equal(blackPlane, _game[Colour.Black]);

        Assert.Equal(pawnPlane, _game[Kind.Pawn]);

        Assert.Equal(rookPlane, _game[Kind.Rook]);

        Assert.Equal(knightPlane, _game[Kind.Knight]);

        Assert.Equal(bishopPlane, _game[Kind.Bishop]);

        Assert.Equal(queenPlane, _game[Kind.Queen]);

        Assert.Equal(kingPlane, _game[Kind.King]);
    }

    [Theory]
    [InlineData(
        0b00000000_00000000_00000000_00000000_00000000_00000000_11111111_11111111,
        0b11111111_11111111_00000000_00000000_00000000_00000000_00000000_00000000,
        0b00000000_11111111_00000000_00000000_00000000_00000000_11111111_00000000,
        0b10000001_00000000_00000000_00000000_00000000_00000000_00000000_10000001,
        0b01000010_00000000_00000000_00000000_00000000_00000000_00000000_01000010,
        0b00100100_00000000_00000000_00000000_00000000_00000000_00000000_00100100,
        0b00001000_00000000_00000000_00000000_00000000_00000000_00000000_00001000,
        0b00010000_00000000_00000000_00000000_00000000_00000000_00000000_00010000)]
    public void CopiesSelfCorrectly(ulong whitePlane, ulong blackPlane, ulong pawnPlane, ulong rookPlane, ulong knightPlane, ulong bishopPlane, ulong queenPlane, ulong kingPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);

        var copy = new Game(_game);

        Assert.Equal(whitePlane, copy[Colour.White]);

        Assert.Equal(blackPlane, copy[Colour.Black]);

        Assert.Equal(pawnPlane, copy[Kind.Pawn]);

        Assert.Equal(rookPlane, copy[Kind.Rook]);

        Assert.Equal(knightPlane, copy[Kind.Knight]);

        Assert.Equal(bishopPlane, copy[Kind.Bishop]);

        Assert.Equal(queenPlane, copy[Kind.Queen]);

        Assert.Equal(kingPlane, copy[Kind.King]);
    }
    
    [Theory]
    [InlineData( 
        0b00000000_00000000_00000000_00000000_00000000_00000001_11111110_11111111,
        0b11111111_11111111_00000000_00000000_00000000_00000000_00000000_00000000)]
    public void UpdatesOnWhiteMoveCorrectly(ulong whitePlane, ulong blackPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(8, 16);
        
        Assert.Equal(whitePlane, _game[Colour.White]);
        
        Assert.Equal(blackPlane, _game[Colour.Black]);
    }
    
    [Theory]
    [InlineData( 
        0b00000000_00000000_00000000_00000000_00000000_00000000_11111111_11111111,
        0b11111111_11111110_00000001_00000000_00000000_00000000_00000000_00000000)]
    public void UpdatesOnBlackMoveCorrectly(ulong whitePlane, ulong blackPlane)
    {
        _game.ParseFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1");
        
        _game.MakeMove(48, 40);
        
        Assert.Equal(whitePlane, _game[Colour.White]);
        
        Assert.Equal(blackPlane, _game[Colour.Black]);
    }

    [Theory]
    [InlineData( 
        0b00000000_11111111_00000000_00000000_00000000_00000001_11111110_00000000)]
    public void UpdatesOnPawnMoveCorrectly(ulong expectedPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(8, 16);
        
        Assert.Equal(expectedPlane, _game[Kind.Pawn]);
    }
    
    [Theory]
    [InlineData( 
        0b10000001_00000000_00000000_00000000_00000000_00000000_00000001_10000000)]
    public void UpdatesOnRookMoveCorrectly(ulong expectedPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(0, 8);
        
        Assert.Equal(expectedPlane, _game[Kind.Rook]);
    }
    
    [Theory]
    [InlineData( 
        0b01000010_00000000_00000000_00000000_00000000_00000000_00001000_01000000)]
    public void UpdatesOnKnightMoveCorrectly(ulong expectedPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(1, 11);
        
        Assert.Equal(expectedPlane, _game[Kind.Knight]);
    }
    
    [Theory]
    [InlineData(
        0b00100100_00000000_00000000_00000000_00000000_00000000_00000010_00100000)]
    public void UpdatesOnBishopMoveCorrectly(ulong expectedPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(2, 9);
        
        Assert.Equal(expectedPlane, _game[Kind.Bishop]);
    }
    
    [Theory]
    [InlineData( 
        0b00001000_00000000_00000000_00000000_00000000_00000000_00000000_00000001)]
    public void UpdatesOnQueenMoveCorrectly(ulong expectedPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(3, 0);
        
        Assert.Equal(expectedPlane, _game[Kind.Queen]);
    }    
    
    [Theory]
    [InlineData("rnbqkbnr/p1p3pp/1p1pp3/5p2/3P1B2/2NQ4/PPP1PPPP/R3KBNR w KQkq - 0 5", "e1c1", Colour.White,
        0b00000000_00000000_00000000_00000000_00101000_00001100_11110111_11101100)]
    [InlineData("rnbqkbnr/ppp3pp/3p1p2/4p3/8/5NPB/PPPPPP1P/RNBQK2R w KQkq - 0 4", "e1g1", Colour.White,
        0b00000000_00000000_00000000_00000000_00000000_11100000_10111111_01101111)]
    [InlineData("rnbqk2r/pppp1ppp/3bp2n/8/8/2PP1P1P/PP2P1P1/RNBQKBNR b KQkq - 0 4", "e8g8", Colour.Black,
        0b01101111_11101111_10011000_00000000_00000000_00000000_00000000_00000000)]
    [InlineData("r3kbnr/pppqpppp/2npb3/8/8/3PPPPP/PPP5/RNBQKBNR b KQkq - 0 5", "e8c8", Colour.Black,
        0b11101100_11111111_00011100_00000000_00000000_00000000_00000000_00000000)]
    public void PerformsCastleCorrectly(string fen, string move, Colour colour, ulong expectedPlane)
    {
        _game.ParseFen(fen);
        
        _game.MakeMove(move[..2].FromStandardNotation(), move[2..].FromStandardNotation());
        
        Assert.Equal(expectedPlane, _game[colour]);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/p7/1K6 w - - 0 1", Colour.White)]
    [InlineData("8/8/8/8/8/8/2p5/1K6 w - - 0 1", Colour.White)]
    [InlineData("1k6/P7/8/8/8/8/8/8 b - - 0 1", Colour.Black)]
    [InlineData("1k6/2P5/8/8/8/8/8/8 b - - 0 1", Colour.Black)]
    [InlineData("1k6/2K5/8/8/8/8/8/8 b - - 0 1", Colour.Black)]
    [InlineData("k7/8/8/8/8/8/8/7B b - - 0 1", Colour.Black)]
    [InlineData("k7/8/8/8/8/8/8/7Q b - - 0 1", Colour.Black)]
    [InlineData("k7/1p6/8/8/8/8/8/7B b - - 0 1", Colour.Black, false)]
    [InlineData("k7/8/2P5/8/8/8/8/7B b - - 0 1", Colour.Black, false)]
    [InlineData("k7/8/8/8/8/8/8/R7 b - - 0 1", Colour.Black)]
    [InlineData("k7/8/8/8/8/8/8/Q7 b - - 0 1", Colour.Black)]
    [InlineData("k7/8/8/8/p7/8/8/R7 b - - 0 1", Colour.Black, false)]
    [InlineData("k7/8/8/8/P7/8/8/R7 b - - 0 1", Colour.Black, false)]
    [InlineData("k7/2N5/8/8/8/8/8/8 b - - 0 1", Colour.Black)]
    public void ReportsKingInCheckCorrectly(string fen, Colour colour, bool check = true)
    {
        _game.ParseFen(fen);

        if (check)
        {
            Assert.True(_game.IsKingInCheck(colour));
        }
        else
        {
            Assert.False(_game.IsKingInCheck(colour));
        }
    }

    [Theory]
    [InlineData("8/8/8/4p3/3P4/8/8/8 w - - 0 1", 27, Colour.Black, true)]
    [InlineData("8/8/8/3p4/3P4/8/8/8 w - - 0 1", 27, Colour.Black, false)]
    [InlineData("8/8/4n3/8/3P4/8/8/8 w - - 0 1", 27, Colour.Black, true)]
    [InlineData("8/8/5n2/8/3P4/8/8/8 w - - 0 1", 27, Colour.Black, false)]
    [InlineData("8/b7/8/8/3P4/8/8/8 w - - 0 1", 27, Colour.Black, true)]
    [InlineData("8/1b6/8/8/3P4/8/8/8 w - - 0 1", 27, Colour.Black, false)]
    [InlineData("8/3r4/8/8/3P4/8/8/8 w - - 0 1", 27, Colour.Black, true)]
    [InlineData("8/4r3/8/8/3P4/8/8/8 w - - 0 1", 27, Colour.Black, false)]
    [InlineData("7q/8/8/8/3P4/8/8/8 w - - 0 1", 27, Colour.Black, true)]
    [InlineData("6q1/8/8/8/3P4/8/8/8 w - - 0 1", 27, Colour.Black, false)]
    [InlineData("3q4/8/8/8/3P4/8/8/8 w - - 0 1", 27, Colour.Black, true)]
    [InlineData("5q2/8/8/8/3P4/8/8/8 w - - 0 1", 27, Colour.Black, false)]
    [InlineData("8/8/8/4k3/3P4/8/8/8 w - - 0 1", 27, Colour.Black, true)]
    [InlineData("8/8/5k2/8/3P4/8/8/8 w - - 0 1", 27, Colour.Black, false)]
    public void DetectsAttackingPiecesCorrectly(string fen, int cell, Colour attackerColour, bool attacked)
    {
        _game.ParseFen(fen);
        
        Assert.Equal(attacked, _game.CellHasAttackers(cell, attackerColour));
    }

    [Theory]
    [InlineData("4k3/1P6/8/8/8/8/8/4K3 w - - 0 1", "b7b8", true)]
    [InlineData("4k3/1P6/8/8/8/8/8/4K3 w - - 0 1", "e1f1", false)]
    public void DetectsPromotion(string fen, string move, bool promotionExpected)
    {
        _game.ParseFen(fen);

        var outcome = _game.MakeMove(move[..2].FromStandardNotation(), move[2..].FromStandardNotation());

        if (promotionExpected)
        {
            Assert.True((outcome & PlyOutcome.Promotion) > 0);
        }
        else
        {
            Assert.False((outcome & PlyOutcome.Promotion) > 0);
        }
    }

    [Theory]
    [InlineData(Constants.InitialBoardFen, null, Scores.Initial, Scores.Initial, Scores.Initial, Scores.Initial)]
    [InlineData("rnbqkbnr/ppp1pppp/8/3p4/2P5/8/PP1PPPPP/RNBQKBNR w KQkq - 0 1", "c4d5", Scores.Initial, Scores.Initial, Scores.Initial, Scores.Initial - Scores.Pawn)]
    [InlineData("rnbqkbnr/ppp1pppp/8/3p4/2P1P3/8/PP1P1PPP/RNBQKBNR b KQkq - 0 1", "d5c4", Scores.Initial, Scores.Initial, Scores.Initial - Scores.Pawn, Scores.Initial)]
    [InlineData("rnbqkbnr/ppp1pppp/8/2Pp4/8/8/PP1PPPPP/RNBQKBNR w KQkq d6 0 1", "c5d6", Scores.Initial, Scores.Initial, Scores.Initial, Scores.Initial - Scores.Pawn)]
    [InlineData("rnbqkbnr/ppp1pppp/8/8/2Pp4/8/PP1PPPPP/RNBQKBNR b KQkq c3 0 1", "d4c3", Scores.Initial, Scores.Initial, Scores.Initial - Scores.Pawn, Scores.Initial)]
    [InlineData("4k3/8/8/4r3/3P4/8/8/4K3 w - - 0 1", "d4e5", Scores.Pawn, Scores.Rook, Scores.Pawn, 0)]
    [InlineData("4k3/8/8/4n3/3P4/8/8/4K3 w - - 0 1", "d4e5", Scores.Pawn, Scores.Knight, Scores.Pawn, 0)]
    [InlineData("4k3/8/8/4b3/3P4/8/8/4K3 w - - 0 1", "d4e5", Scores.Pawn, Scores.Bishop, Scores.Pawn, 0)]
    [InlineData("4k3/8/8/4q3/3P4/8/8/4K3 w - - 0 1", "d4e5", Scores.Pawn, Scores.Queen, Scores.Pawn, 0)]
    [InlineData("4k3/8/8/4R3/3p4/8/8/4K3 b - - 0 1", "d4e5", Scores.Rook, Scores.Pawn, 0, Scores.Pawn)]
    [InlineData("4k3/8/8/4N3/3p4/8/8/4K3 b - - 0 1", "d4e5", Scores.Knight, Scores.Pawn, 0, Scores.Pawn)]
    [InlineData("4k3/8/8/4B3/3p4/8/8/4K3 b - - 0 1", "d4e5", Scores.Bishop, Scores.Pawn, 0, Scores.Pawn)]
    [InlineData("4k3/8/8/4Q3/3p4/8/8/4K3 b - - 0 1", "d4e5", Scores.Queen, Scores.Pawn, 0, Scores.Pawn)]
    public void HandlesScoringCorrectly(string fen, string move, int initialWhiteScore, int initialBlackScore, int expectedWhiteScore, int expectedBlackScore)
    {
        var game = new Game();
        
        game.ParseFen(fen);
        
        Assert.Equal(initialWhiteScore, game.State.WhiteScore);
        
        Assert.Equal(initialBlackScore, game.State.BlackScore);

        if (move != null)
        {
            game.MakeMove(move[..2].FromStandardNotation(), move[2..].FromStandardNotation());
        }
        
        Assert.Equal(expectedWhiteScore, game.State.WhiteScore);
        
        Assert.Equal(expectedBlackScore, game.State.BlackScore);
    }

    [Fact]
    public void ThrowsExceptionOnMoveIfNoPieceInFromCell()
    {
        _game.ParseFen(Constants.InitialBoardFen);

        var exception = Assert.Throws<InvalidMoveException>(() => _game.MakeMove(16, 17));
        
        Assert.Equal("No piece at a3.", exception.Message);
    }
    
    [Fact]
    public void ThrowsExceptionIfWrongPlayerMoves()
    {
        _game.ParseFen(Constants.InitialBoardFen);

        var exception = Assert.Throws<InvalidMoveException>(() => _game.MakeMove(48, 49));
        
        Assert.Equal("Not the turn for Black.", exception.Message);
    }
    
    [Fact]
    public void ThrowsExceptionIfNoPieceAtCell()
    {
        _game.ParseFen(Constants.InitialBoardFen);

        var exception = Assert.Throws<InvalidMoveException>(() => _game.GetKind(16));
        
        Assert.Equal("No piece at a3.", exception.Message);
    }
}