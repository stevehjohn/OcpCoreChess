// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace OcpCore.Engine.LiChessClient.Client.Models;

[UsedImplicitly]
public class StreamResponse
{
    [JsonPropertyName("white")]
    public Player White { get; init; }
    
    [JsonPropertyName("black")]
    public Player Black { get; init; }
    
    [JsonPropertyName("state")]
    public GameState State { get; init; }
}