using System.Text.Json;
using System.Text;
using Dalana.Models;
using Spectre.Console;
using Dalana.Services;

string apiUrl = "http://localhost:11434/api/generate";

using HttpClient httpClient = new HttpClient();

Console.WriteLine("🟢 AI-core activated write 'exit' to turn off tool\n");

SystemController.OpenSmart("");

while (true)
{
    Console.Write("What's your request today?: ");
    string userInput = Console.ReadLine() ?? "";
    
    if (userInput.ToLower() == "exit" || userInput.ToLower() == "quit")
    {
        Console.WriteLine();
        
        AnsiConsole.Write(new Rule("[red]System Shutdown[/]").RuleStyle("grey").Centered());
        
        AnsiConsole.Write(
            new FigletText("OFFLINE")
                .Centered() 
                .Color(Color.Red)); 

        Console.WriteLine();
        break; 
    }
    
    if (string.IsNullOrEmpty(userInput)) continue;

    OllamaRequest requestData = new OllamaRequest
    {
        Model = "llama3",
        Prompt = userInput,
        Stream = false
    };

    string jsonPayload = JsonSerializer.Serialize(requestData);

    StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

    HttpResponseMessage? response = null;

    await AnsiConsole.Status()
        .Spinner(Spinner.Known.BouncingBar) 
        .SpinnerStyle(Style.Parse("green")) 
        .StartAsync("Sending your request...", async ctx =>
        {
            response = await httpClient.PostAsync(apiUrl, content);
        });

    Console.WriteLine();
    
    string rawResult = await response.Content.ReadAsStringAsync();
    OllamaResponse? aiData = JsonSerializer.Deserialize<OllamaResponse>(rawResult);

    if (aiData != null)
    {
        var panel = new Panel(aiData.Response)
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1, 1, 1)
        };
        panel.Header("[blue]Answer: [/]");
    
        AnsiConsole.Write(panel);
        Console.WriteLine();
    }
}