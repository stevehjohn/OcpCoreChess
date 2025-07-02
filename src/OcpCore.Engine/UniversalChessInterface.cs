using Engine.Infrastructure;

namespace Engine;

public sealed class UniversalChessInterface : IDisposable
{
    private const int DefaultDepth = 6;
    
    private readonly Core _core = new();

    private readonly Action<string> _responseCallback;
    
    public UniversalChessInterface(Action<string> responseCallback)
    {
        _responseCallback = responseCallback;
    }
    
    public void IssueCommand(string command)
    {
        ParseCommand(command);
    }

    private void ParseCommand(string command)
    {
        command = command.ToLower();
        
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        switch (parts[0])
        {
            case "uci":
                Uci();
                
                break;
            
            case "ucinewgame":
                UciNewGame();

                break;
            
            case "isready":
                IsReady();

                break;
            
            case "position":
                Position(parts[1..]);
                
                break;
            
            case "go":
                Go(parts.Length > 1 ? parts[1..] : null);

                break;
            
            case "stop":
                Stop();
                
                break;
            
            case "quit":
                Quit();

                break;
            
            default:
                throw new EngineException($"Unknown UCI command '{command}'.");
        }
    }

    private void Uci()
    {
        Reply($"id name {Core.EngineName}");
        
        Reply($"id author {Core.Author}");
        
        Reply("uciok");
    }

    private void UciNewGame()
    {
        _core.Initialise();
    }

    private void IsReady()
    {
        Reply("readyok");
    }

    private void Position(string[] commands)
    {
        if (commands[0] == "startpos")
        {
            _core.Initialise();
        }
        else
        {
            throw new EngineException("Only 'startpos' position is currently supported.");
        }

        if (commands.Length == 1)
        {
            return;
        }

        if (commands[1] != "moves")
        {
            throw new EngineException("Expected 'moves' or no further instructions.");
        }

        for (var i = 2; i < commands.Length; i++)
        {
            try
            {
                _core.MakeMove(commands[i]);
            }
            catch (Exception exception)
            {
                throw new EngineException($"An error occurred trying to make move {commands[i]}. {exception}");
            }
        }
    }

    private void Go(string[] commands)
    {
        var depth = DefaultDepth;

        var moveTime = 0;
        
        if (commands != null)
        {
            for (var i = 0; i < commands.Length; i += 2)
            {
                switch (commands[i])
                {
                    case "depth":
                        if (! int.TryParse(commands[i + 1], out depth))
                        {
                            throw new EngineException($"Cannot parse go depth parameter '{commands[i + 1]}'.");
                        }

                        break;
                    
                    case "movetime":
                        if (! int.TryParse(commands[i + 1], out moveTime))
                        {
                            throw new EngineException($"Cannot parse go movetime parameter '{commands[i + 1]}'.");
                        }

                        break;
                    
                    case "wtime":
                    case "btime":
                    case "winc":
                    case "binc":
                        break;

                    default:
                        throw new EngineException($"Unsupported go parameter '{commands[i]}'.");
                }
            }
        }

        _core.GetMove(depth, move => _responseCallback($"bestmove {move}"), moveTime);
    }

    private void Stop()
    {
        Reply($"bestmove {_core.Interrupt()}");
    }

    private void Quit()
    {
        if (_core.IsBusy)
        {
            try
            {
                _core.Interrupt();
            }
            catch
            {
                //
            }
        }
    }

    private void Reply(string response)
    {
        _responseCallback(response);
    }

    public void Dispose()
    {
        _core?.Dispose();
    }
}