using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dalana.Models;

namespace Dalana.Services;

public static class OllamaService
{
    private static readonly HttpClient httpClient = new HttpClient();
    private const string ollamaUrl = "http://localhost:11434/api/chat";

    public static async Task<string> GetLocalResponseAsync(List<ChatMessage> history)
    {
        var requestData = new
        {
            model = "llama3",
            message = history,
            stream = false,
            format = "json"
        };

        string jsonPayload = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        try
        {
            HttpResponseMessage response = await httpClient.PostAsync(ollamaUrl, content);
            response.EnsureSuccessStatusCode();

            string rawResult = await response.Content.ReadAsStringAsync();

            // Парсимо відповідь Ollama
            using JsonDocument doc = JsonDocument.Parse(rawResult);
            JsonElement root = doc.RootElement;

            // Дістаємо текст повідомлення з JSON
            return root.GetProperty("message").GetProperty("content").GetString() ?? "";
        }
        catch (Exception ex)
        {
            return $"[ERROR] Ollama failed with this message: {ex.Message}";
        }
    }
}

