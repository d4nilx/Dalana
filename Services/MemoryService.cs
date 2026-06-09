using System.Text.Json;
using Dalana.Models;

namespace Dalana.Services;

public static class MemoryService
{
    private static readonly string MemoryFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
        ".dalana_history.json"
    );

    public static void SaveHistory(List<ChatMessage> history)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(history, options);
        
        File.WriteAllText(MemoryFilePath, json);
    }

    public static List<ChatMessage> LoadHistory()
    {
        if (!File.Exists(MemoryFilePath))
        {
            return new List<ChatMessage>();
        }

        string json = File.ReadAllText(MemoryFilePath);
        return JsonSerializer.Deserialize<List<ChatMessage>>(json) ?? new List<ChatMessage>();
    }
}