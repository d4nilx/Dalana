using System.Text.Json.Serialization;

namespace Dalana.Models;

public class GroqAction
{
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty; 

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty; 
    
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    [JsonPropertyName("target_app")]
    public string? TargetApp { get; set; }
}