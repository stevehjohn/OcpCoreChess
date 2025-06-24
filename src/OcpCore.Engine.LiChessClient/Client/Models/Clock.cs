// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace OcpCore.Engine.LiChessClient.Client.Models;

[UsedImplicitly]
public class Clock
{
    [JsonPropertyName("limit")]
    public int Linit { get; set; }
    
    [JsonPropertyName("increment")]
    public int Increment { get; set; }
}