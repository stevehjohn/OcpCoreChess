// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace OcpCore.Engine.LiChessClient.Client.Models;

[UsedImplicitly]
public class GameState
{
    [JsonPropertyName("moves")]
    public string Moves { get; init; }
    
    [JsonPropertyName("text")]
    public string Text { get; init; }
    
    [JsonPropertyName("winner")]
    public string Winner { get; init; }
}