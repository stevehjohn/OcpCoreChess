using System.Text.Json.Serialization;

namespace OcpCore.LichessClient.Client.Models;

public class ChatRequest
{
    [JsonPropertyName("room")]
    public string Room { get; set; }
    
    [JsonPropertyName("text")]
    public string Text { get; set; }
}