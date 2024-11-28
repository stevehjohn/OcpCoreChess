using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;
using OcpCore.Engine.Tests.Infrastructure;
using Xunit;

namespace OcpCore.Engine.Tests.General;

public class StateTests
{
    [Theory]
    [Repeat(10, nameof(State.InvertPlayer))]
    [Repeat(10, nameof(State.SetEnPassantTarget))]
    [Repeat(10, nameof(State.RemoveCastleRights))]
    [Repeat(10, nameof(State.UpdateWhiteScore))]
    [Repeat(10, nameof(State.UpdateBlackScore))]
    [Repeat(10, nameof(State.SetWhiteKingCell))]
    [Repeat(10, nameof(State.SetBlackKingCell))]
    public void StateComponentsDoNotInterfereWithEachOther(string methodToInvoke)
    {
        var rng = Random.Shared;

        var player = rng.Next(2) == 1 ? Colour.White : Colour.Black;

        var castleStatus = rng.Next(6) switch
        {
            1 => Castle.WhiteQueenSide,
            2 => Castle.WhiteKingSide,
            3 => Castle.BlackQueenSide,
            4 => Castle.BlackKingSide,
            5 => Castle.All,
            _ => Castle.NotAvailable
        };

        var enPassantTarget = rng.Next(Constants.Cells);

        var whiteScore = rng.Next(Scores.Initial);

        var blackScore = rng.Next(Scores.Initial);

        var whiteKingCell = rng.Next(Constants.Cells);

        var blackKingCell = rng.Next(Constants.Cells);

        var state = new State(player, castleStatus, enPassantTarget, whiteScore, blackScore, whiteKingCell, blackKingCell);

        Assert.Equal(player, state.Player);

        Assert.Equal(castleStatus, state.CastleStatus);

        Assert.Equal(enPassantTarget, state.EnPassantTarget);

        Assert.Equal(whiteScore, state.WhiteScore);

        Assert.Equal(blackScore, state.BlackScore);

        Assert.Equal(whiteKingCell, state.WhiteKingCell);

        Assert.Equal(blackKingCell, state.BlackKingCell);

        var property = state.GetType().GetMethod(methodToInvoke)!;

        var whiteScoreDelta = 0;

        var blackScoreDelta = 0;

        switch (methodToInvoke)
        {
            case nameof(State.SetEnPassantTarget):
                enPassantTarget = rng.Next(Constants.Cells);

                property.Invoke(state, [enPassantTarget]);

                break;

            case nameof(State.RemoveCastleRights):

                castleStatus = rng.Next(5) switch
                {
                    1 => Castle.WhiteQueenSide,
                    2 => Castle.WhiteKingSide,
                    3 => Castle.BlackQueenSide,
                    4 => Castle.BlackKingSide,
                    _ => Castle.All
                };

                property.Invoke(state, [castleStatus]);

                break;

            case nameof(State.UpdateWhiteScore):
                whiteScoreDelta = rng.Next(Scores.Initial);

                property.Invoke(state, [whiteScoreDelta]);

                break;

            case nameof(State.UpdateBlackScore):
                blackScoreDelta = rng.Next(Scores.Initial);

                property.Invoke(state, [blackScoreDelta]);

                break;

            case nameof(State.SetWhiteKingCell):
                whiteKingCell = rng.Next(Constants.Cells);

                property.Invoke(state, [whiteKingCell]);

                break;

            case nameof(State.SetBlackKingCell):
                blackKingCell = rng.Next(Constants.Cells);

                property.Invoke(state, [blackKingCell]);

                break;

            default:
                property.Invoke(state, null);

                break;
        }

        switch (methodToInvoke)
        {
            case nameof(State.InvertPlayer):
                Assert.Equal(player == Colour.White ? Colour.Black : Colour.White, state.Player);

                Assert.Equal(castleStatus, state.CastleStatus);

                Assert.Equal(enPassantTarget, state.EnPassantTarget);

                Assert.Equal(whiteScore, state.WhiteScore);

                Assert.Equal(blackScore, state.BlackScore);

                Assert.Equal(whiteKingCell, state.WhiteKingCell);

                Assert.Equal(blackKingCell, state.BlackKingCell);

                break;

            case nameof(State.RemoveCastleRights):
                Assert.Equal(player, state.Player);

                Assert.Equal(Castle.NotAvailable, castleStatus & state.CastleStatus);

                Assert.Equal(enPassantTarget, state.EnPassantTarget);

                Assert.Equal(whiteScore, state.WhiteScore);

                Assert.Equal(blackScore, state.BlackScore);

                Assert.Equal(whiteKingCell, state.WhiteKingCell);

                Assert.Equal(blackKingCell, state.BlackKingCell);

                break;

            case nameof(State.SetEnPassantTarget):
                Assert.Equal(player, state.Player);

                Assert.Equal(castleStatus, state.CastleStatus);

                Assert.Equal(enPassantTarget, state.EnPassantTarget);

                Assert.Equal(whiteScore, state.WhiteScore);

                Assert.Equal(blackScore, state.BlackScore);

                Assert.Equal(whiteKingCell, state.WhiteKingCell);

                Assert.Equal(blackKingCell, state.BlackKingCell);

                break;

            case nameof(State.UpdateWhiteScore):
            case nameof(State.UpdateBlackScore):
                Assert.Equal(player, state.Player);

                Assert.Equal(castleStatus, state.CastleStatus);

                Assert.Equal(enPassantTarget, state.EnPassantTarget);

                Assert.Equal(whiteScore + whiteScoreDelta, state.WhiteScore);

                Assert.Equal(blackScore + blackScoreDelta, state.BlackScore);

                Assert.Equal(whiteKingCell, state.WhiteKingCell);

                Assert.Equal(blackKingCell, state.BlackKingCell);

                break;

            case nameof(State.SetWhiteKingCell):
            case nameof(State.SetBlackKingCell):
                Assert.Equal(player, state.Player);

                Assert.Equal(castleStatus, state.CastleStatus);

                Assert.Equal(enPassantTarget, state.EnPassantTarget);

                Assert.Equal(whiteScore, state.WhiteScore);

                Assert.Equal(blackScore, state.BlackScore);

                Assert.Equal(whiteKingCell, state.WhiteKingCell);

                Assert.Equal(blackKingCell, state.BlackKingCell);

                break;
        }
    }
}