using System.Text.Json.Serialization;

namespace Dalana.Models;

public class GroqResponse
{
    [JsonPropertyName("choices")]
    public List<GroqChoice> Choices { get; set; } = new();
}

public class GroqChoice
{
    [JsonPropertyName("message")]
    public ChatMessage? Message { get; set; }
}