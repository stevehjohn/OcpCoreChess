using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Engine.General;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.LiChessClient.Client.Models;
using OcpCore.Engine.LiChessClient.Exceptions;
using static OcpCore.Engine.LiChessClient.Infrastructure.Console;

namespace OcpCore.Engine.LiChessClient.Client;

public sealed class LiChessClient : IDisposable
{
    private const int WaitAttempts = 6;

    private const int Depth = 5;
    
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _serializerOptions;

    private readonly bool _logCommunications;
    
    private Core _core;
    
    public LiChessClient(bool logCommunications = false)
    {
        _client = new HttpClient();

        var apiKey = File.ReadAllLines("LiChess.key")[0];

        _client = new HttpClient
        {
            BaseAddress = new Uri("https://lichess.org")
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        _logCommunications = logCommunications;

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            WriteIndented = true
        };
    }

    public async Task<int> ChallengeLiChess(string username, bool rated)
    {
        OutputLine($"&NL;  &Cyan;Challenging &White;{username}");

        var response = await Post<ChallengeRequest, ChallengeResponse>($"challenge/{username}", new ChallengeRequest
        {
            Clock = new Clock
            {
                Increment = 10,
                Linit = 900
            },
            KeepAliveStream = false,
            Rated = rated,
            Variant = "standard"
        });

        switch (response.Status)
        {
            case "created":
                OutputLine("&NL;  &Cyan;Challenge &Yellow;CREATED&White;.");

                var challengeState = await AwaitAcceptance(response.Id);

                if (challengeState.Accepted)
                {
                    OutputLine("&NL;  &Cyan;Challenge &Green;ACCEPTED&White;.");

                    return await PlayGame(response.Id);
                }

                OutputLine($"&NL;  &Cyan;Challenge &Magenta;{challengeState.Reason}&White;.");
                break;

            case "accepted":
                OutputLine("&NL;  &Cyan;Challenge &Green;ACCEPTED&White;.");

                return await PlayGame(response.Id);
            
            default:
                OutputLine("&NL;  &Cyan;Challenge &Magenta;DECLINED&White;.");
                break;
        }
        
        OutputLine();

        return 0;
    }

    private async Task<(bool Accepted, string Reason)> AwaitAcceptance(string id)
    {
        OutputLine($"&NL;  &Cyan;Game ID: &White;{id}");
        
        Thread.Sleep(1000);

        for (var attempt = 1; attempt <= WaitAttempts; attempt++)
        {
            var response = await Get<ChallengeResponse>($"challenge/{id}/show");

            switch (response.Status)
            {
                case "accepted":
                    return (true, null);
                
                case "declined" or "offline":
                    return (false, response.Status.ToUpperInvariant());
                
                case "created":
                    Output($"&NL;  &Cyan;Attempt &White;{attempt}&Cyan; of &White;{WaitAttempts}&Cyan; Waiting ");

                    var y = CursorLeft;
                
                    for (var i = 10; i >= 0; i--)
                    {
                        CursorLeft = y;
                    
                        switch (i)
                        {
                            case > 5:
                                Output("&Magenta;");
                                break;
                            case > 2:
                                Output("&Yellow;");
                                break;
                            default:
                                Output("&Green;");
                                break;
                        }

                        Output($"{i}  ");

                        Thread.Sleep(1000);
                    }

                    break;
            }

            CursorLeft = 0;
        }

        return (false, "TIMEOUT");
    }

    private async Task<int> PlayGame(string id)
    {
        using var response = await _client.GetAsync($"api/bot/game/stream/{id}", HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();

        using var reader = new StreamReader(stream);

        var first = true;

        var engineIsWhite = true;

        Process.Start("open", $"https://lichess.org/{id}");

        var opponentName = string.Empty;

        while (! reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
            {
                OutputLine("  &White;...");

                continue;
            }
            
            GameState state;
            
            if (first)
            {
                first = false;

                var game = JsonSerializer.Deserialize<StreamResponse>(line);

                OutputLine($"&NL;  &Cyan;White&White;: &Green;{game.White.Name}    &Cyan;Black&White;: &Green;{game.Black.Name}");
                
                engineIsWhite = game.White.Name == "StevoJ";

                opponentName = engineIsWhite ? game.Black.Name : game.White.Name;

                state = game.State;

                _core = new Core(Colour.White);
            }
            else
            {
                state = JsonSerializer.Deserialize<GameState>(line);

                if (! string.IsNullOrWhiteSpace(state.Text))
                {
                    OutputLine($"&NL;  &Magenta;{opponentName}&White;: &Yellow;{state.Text}");
                    
                    continue;
                }

                _core = new Core(Colour.Black);
            }

            if (state == null)
            {
                continue;
            }

            var result = await PlayMove(id, state, engineIsWhite);

            if (result != 0)
            {
                return result;
            }
        }

        return 0;
    }

    private async Task<int> PlayMove(string id, GameState state, bool engineIsWhite)
    {
        var moves = (state.Moves ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var lastMove = string.Empty;

        if (moves.Length > 0)
        {
            lastMove = moves[^1];
        }

        if (_core.Player == Colour.White && engineIsWhite)
        {
            OutputLine("&NL;  &Cyan;Thinking&White;...");
            
            var engineMove = _core.GetMove(Depth);

            OutputBestScores();
            
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (engineMove.Outcome)
            {
                case MoveOutcome.EngineInCheckmate:
                    OutputLine("&NL;  &Magenta;Got nothing :(&White;...");
            
                    return -1;
                
                case MoveOutcome.OpponentInCheckmate:
                    await Post<NullRequest, BasicResponse>($"bot/game/{id}/move/{engineMove.Move}", null);
                    
                    OutputLine("&NL;  &Green;Checkmate :)&White;...");

                    return 1;
                
                // case PlyOutcome.Stalemate:
                //     OutputLine("&NL;  &Gray;Stalemate...");

                    return 0;
            }

            OutputLine($"&NL;  &Green;Engine&White;: {engineMove}");
            
            var result = await Post<NullRequest, BasicResponse>($"bot/game/{id}/move/{engineMove.Move}", null);

            if (! result.Ok)
            {
                throw new ClientException("Error communicating with LiChess API.");
            }

            _core.MakeMove(engineMove.Move);
            
            OutputLine();
            
            _core.OutputBoard();
        }
        else
        {
            _core.MakeMove(lastMove);

            OutputLine($"&NL;  &Green;Opponent&White;: {lastMove}");
                        
            OutputLine();
            
            _core.OutputBoard(! engineIsWhite);

            OutputLine("&NL;  &Cyan;Thinking&White;...");
            
            var engineMove = _core.GetMove(Depth);
            
            OutputBestScores();

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (engineMove.Outcome)
            {
                case MoveOutcome.EngineInCheckmate:
                    OutputLine("&NL;  &Magenta;Got nothing :(&White;...");
            
                    return -1;
                case MoveOutcome.OpponentInCheckmate:
                    await Post<NullRequest, BasicResponse>($"bot/game/{id}/move/{engineMove.Move}", null);
                    
                    OutputLine("&NL;  &Green;Checkmate :)&White;...");

                    return 1;
                
                // case PlyOutcome.Stalemate:
                //     OutputLine("&NL;  &Gray;Stalemate...");

                    return 0;
            }

            OutputLine($"&NL;  &Green;Engine&White;: {engineMove}");
            
            var result = await Post<NullRequest, BasicResponse>($"bot/game/{id}/move/{engineMove.Move}", null);
            
            if (! result.Ok)
            {
                throw new ClientException("Error communicating with LiChess API.");
            }

            _core.MakeMove(engineMove.Move);

            OutputLine();
            
            _core.OutputBoard(! engineIsWhite);
        }

        return 0;
    }

    private void OutputBestScores()
    {
        OutputLine();
        
        for (var i = 0; i < Depth; i++)
        {
            //OutputLine($"  &Cyan;Ply&White;: {i + 1}  &Cyan;Best Score&White;: {_core.GetBestScore(i + 1)}");
        }
    }

    private async Task<TResponse> Post<TRequest, TResponse>(string path, TRequest content) where TRequest : class
    {
        var tries = 3;

        Exception lastException = null;
        
        while (tries > 0)
        {
            try
            {
                if (_logCommunications)
                {
                    OutputLine($"&NL;&Gray;POST: api/{path}");
                }

                using var response = await _client.PostAsync($"api/{path}", content is NullRequest ? new StringContent(string.Empty) : JsonContent.Create(content));

                if (_logCommunications)
                {
                    OutputLine($"&NL;{response.StatusCode}");
                }

                var responseString = await response.Content.ReadAsStringAsync();

                if (! response.IsSuccessStatusCode)
                {
                    OutputLine($"&NL;&Magenta;{response.StatusCode}");
                    OutputLine($"&Gray;{JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonElement>(responseString), _serializerOptions)}");
                }

                var responseObject = JsonSerializer.Deserialize<TResponse>(responseString);

                if (_logCommunications)
                {
                    OutputLine($"&Gray;{JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonElement>(responseString), _serializerOptions)}");
                }

                return responseObject;
            }
            catch (Exception exception)
            {
                lastException = exception;
                
                tries--;
                
                Thread.Sleep(1_000);
            }
        }

        throw lastException ?? new Exception("Something has gone wrong.");
    }

    private async Task<TResponse> Get<TResponse>(string path)
    {
        var tries = 3;

        Exception lastException = null;

        while (tries > 0)
        {
            try
            {
                if (_logCommunications)
                {
                    OutputLine();

                    OutputLine($"&Gray;GET: api/{path}");
                }

                using var response = await _client.GetAsync($"api/{path}");

                if (_logCommunications)
                {
                    OutputLine($"&NL;{response.StatusCode}");
                }

                var responseString = await response.Content.ReadAsStringAsync();

                var responseObject = JsonSerializer.Deserialize<TResponse>(responseString);

                if (_logCommunications)
                {
                    OutputLine($"&Gray;{JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonElement>(responseString), _serializerOptions)}");

                    OutputLine();
                }

                return responseObject;
            }
            catch (Exception exception)
            {
                lastException = exception;

                tries--;

                Thread.Sleep(1_000);
            }
        }

        throw lastException ?? new Exception("Something has gone wrong.");
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}