using System.Text.Json;
using System.Text;
using Dalana.Models;

string apiUrl = "http://localhost:11434/api/generate";

Console.Write("What's your request today?: ");
string userInput = Console.ReadLine() ?? "";

OllamaRequest requestData = new OllamaRequest
{
    Model = "llama3",
    Prompt = userInput,
    Stream = false
};

string jsonPayload = JsonSerializer.Serialize(requestData); 

using HttpClient httpClient = new HttpClient();
StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

Console.Write("\nSending your request");

var requestTask = httpClient.PostAsync(apiUrl, content);

while (!requestTask.IsCompleted)
{
    Console.Write(".");
    await Task.Delay(500);
}

HttpResponseMessage response = await requestTask;
Console.WriteLine("\n");

string rawResult = await response.Content.ReadAsStringAsync();

OllamaResponse? aiData = JsonSerializer.Deserialize<OllamaResponse>(rawResult);

if (aiData != null)
{
    Console.WriteLine("Answer: ");
    Console.WriteLine(aiData.Response);
}