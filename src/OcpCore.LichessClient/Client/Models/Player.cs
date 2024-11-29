// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace OcpCore.LichessClient.Client.Models;

[UsedImplicitly]
public class Player
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}