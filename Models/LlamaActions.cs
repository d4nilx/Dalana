using System.Text.Json.Serialization;

namespace Dalana.Models;

public class LlamaAction
{
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty; 

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty; 
}