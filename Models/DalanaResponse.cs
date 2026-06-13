using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Dalana.Models;

public class DalanaResponse
{
    [JsonPropertyName("thought")]
    public string Thought { get; set; } = string.Empty;

    [JsonPropertyName("actions")]
    public List<GroqAction> Actions { get; set; } = new();
}