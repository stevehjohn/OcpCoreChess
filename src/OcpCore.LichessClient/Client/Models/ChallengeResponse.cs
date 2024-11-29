// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace OcpCore.LichessClient.Client.Models;

[UsedImplicitly]
public class ChallengeResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; }
}