using System.Text.Json.Serialization;

namespace Dalana.Models;

public class OllamaResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;
}