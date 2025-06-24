// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace OcpCore.Engine.LiChessClient.Client.Models;

[UsedImplicitly]
public class ChallengeRequest
{
    [JsonPropertyName("clock")]
    public Clock Clock { get; set; }
    
    [JsonPropertyName("keepAliveStream")]
    public bool KeepAliveStream { get; set; }
    
    [JsonPropertyName("rated")]
    public bool Rated { get; set; }

    [JsonPropertyName("variant")]
    public string Variant { get; set; }
}