using System.Text.Json;
using System.Text;
using Dalana.Models;
using Spectre.Console;
using Dalana.Services;
using DotNetEnv;

using HttpClient httpClient = new HttpClient();
Env.TraversePath().Load();

string groqApiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY") ?? "";

if (string.IsNullOrEmpty(groqApiKey))
{
    AnsiConsole.MarkupLine("[bold red]ERROR:[/] API key is not in file .env!");
    return;
}
string apiUrl = "https://api.groq.com/openai/v1/chat/completions";
httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {groqApiKey}");

List<ChatMessage> conversationHistory = MemoryService.LoadHistory();

if (conversationHistory.Count == 0)
{
    conversationHistory.Add(new ChatMessage
    {
        Role = "system",
        Content = @"You are Dalana, a strict macOS CLI system controller and intelligent assistant.
You MUST respond ONLY with a valid JSON object. You can ask follow-up questions ONLY in conversation. NEVER explain anything outside JSON.

Rules:
1. Action 'open': use ONLY for websites, URLs, or Google searches. Format: { ""action"": ""open"", ""value"": ""<target>"" }
2. Action 'open_app': use ONLY for launching local macOS applications. Format: { ""action"": ""open_app"", ""value"": ""<app_name>"" }
3. Action 'chat': use ONLY for general questions, advice, ideas, conversations. Format: { ""action"": ""chat"", ""value"": ""<your_answer>"" }
4. Action 'create_file': use to generate files. You MUST include 'target_app' IF the user specifically asks to open it in a certain program (like PyCharm, Rider, Word). Format: { """"action"""": """"create_file"""", """"value"""": """"<file_name>"""", """"content"""": """"<code_or_text>"""", """"target_app"""": """"<app_name>"" }
5. Action 'open_file': use ONLY to open existing local files. Format: { """"action"""": """"open_file"""", """"value"""": """"<file_name_with_extension>"""" }

CRITICAL EXAMPLES TO FOLLOW:
User: 'youtube' -> { """"action"""": """"open"""", """"value"""": """"youtube.com"""" }
ser: 'write python script and open in PyCharm' -> { """"action"""": """"create_file"""", """"value"""": """"script.py"""", """"content"""": """"print(1)"""", """"target_app"""": """"PyCharm"""" }
User: 'create a text document' -> { """"action"""": """"create_file"""", """"value"""": """"doc.txt"""", """"content"""": """"hello"""", """"target_app"""": """""""" }
User: 'open booking site' -> { ""action"": ""open"", ""value"": ""booking.com"" }
User: 'open app Calculator' -> { ""action"": ""open_app"", ""value"": ""Calculator"" }
User: 'launch rider' -> { ""action"": ""open_app"", ""value"": ""Rider"" }
User: 'open grocery_list.txt' -> { """"action"""": """"open_file"""", """"value"""": """"grocery_list.txt"""" }
User: 'what is async await' -> { ""action"": ""chat"", ""value"": ""async/await is..."" }"
    });

    MemoryService.SaveHistory(conversationHistory);
}

Console.Clear();
AnsiConsole.Write(
    new FigletText("DALANA")
        .LeftJustified()
        .Color(Color.SpringGreen3));
AnsiConsole.MarkupLine(" [grey]AI-Core [bold green]ONLINE[/]. Type [bold red]exit[/] to shut down.[/]\n");
AnsiConsole.Write(new Rule().RuleStyle("grey"));
Console.WriteLine();

while (true)
{
    string userInput = AnsiConsole.Prompt(
        new TextPrompt<string>("[bold cyan]❯ You:[/]")
            .PromptStyle("white")
            .AllowEmpty()
    ) ?? "";

    if (userInput.ToLower() == "exit" || userInput.ToLower() == "quit")
    {
        Console.WriteLine();
        AnsiConsole.Write(new Rule("[red]System Shutdown[/]").RuleStyle("grey").Centered());
        AnsiConsole.Write(new FigletText("OFFLINE").Centered().Color(Color.Red));
        break;
    }

    if (string.IsNullOrWhiteSpace(userInput)) continue;

    conversationHistory.Add(new ChatMessage
    {
        Role = "user",
        Content = userInput
    });
    

    GroqRequest requestData = new GroqRequest
    {
        Model = "llama-3.3-70b-versatile",
        Messages = conversationHistory,
        Stream = false
    };

    string jsonPayload = JsonSerializer.Serialize(requestData);
    StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

    HttpResponseMessage? response = null;

    await AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots) 
        .SpinnerStyle(Style.Parse("green"))
        .StartAsync("Dalana is thinking...", async ctx =>
        {
            response = await httpClient.PostAsync(apiUrl, content);
        });

    Console.WriteLine();

    string rawResult = await response!.Content.ReadAsStringAsync();
    
    if (!response.IsSuccessStatusCode)
    {
        AnsiConsole.MarkupLine($"[bold red]⚠️ API Error ({response.StatusCode}):[/]");
        Console.WriteLine(rawResult);
        continue; 
    }
    
    GroqResponse? aiData = JsonSerializer.Deserialize<GroqResponse>(rawResult);

    if (aiData?.Choices?.Count > 0 && aiData.Choices[0].Message != null)
    {
        var message = aiData.Choices[0].Message!;
        
        conversationHistory.Add(new ChatMessage
        {
            Role = "assistant",
            Content = message.Content
        });

        try
        {
            string cleanJson = message.Content.Trim();
            int start = cleanJson.IndexOf('{');
            int end = cleanJson.LastIndexOf('}');
            if (start >= 0 && end > start)
                cleanJson = cleanJson.Substring(start, end - start + 1);

            GroqAction? actionData = JsonSerializer.Deserialize<GroqAction>(cleanJson);

            if (actionData != null)
            {
                if (actionData.Action == "open")
                {
                    var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.Green);
                    table.AddColumn("[bold green]🛠 System Execution[/]");
                    table.AddRow($"Opening target: [yellow]{Markup.Escape(actionData.Value)}[/]");
                    AnsiConsole.Write(table);
                    
                    SystemController.OpenSmart(actionData.Value);
                }
                else if (actionData.Action == "open_app") 
                {
                    var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.Purple);
                    table.AddColumn("[bold purple]🚀 App Launcher[/]");
                    table.AddRow($"Starting application: [yellow]{Markup.Escape(actionData.Value)}[/]");
                    AnsiConsole.Write(table);
                    
                    SystemController.OpenApp(actionData.Value);
                }
                else if (actionData.Action == "open_file")
                {
                    var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.Yellow);
                    table.AddColumn("[bold yellow]📂 File Opener[/]");
                    table.AddRow($"Opening file: [green]{Markup.Escape(actionData.Value)}[/]");
                    AnsiConsole.Write(table);
                    
                    SystemController.OpenFile(actionData.Value);
                }
                else if (actionData.Action == "chat")
                {
                    var panel = new Panel(Markup.Escape(actionData.Value))
                    {
                        Border = BoxBorder.Rounded,
                        Padding = new Padding(1, 1, 1, 1),
                        // BorderColor = Color.Blue
                    };
                    panel.Header("[blue]Dalana AI[/]");
                    AnsiConsole.Write(panel);
                }
                else if (actionData.Action == "create_file")
                {
                    var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.Orange1);
                    table.AddColumn("[bold orange1]📄 File Generator[/]");
                    table.AddRow($"Creating file: [yellow]{Markup.Escape(actionData.Value)}[/]");
                    table.AddRow($"Writing [green]{actionData.Content?.Length ?? 0}[/] characters...");
                    AnsiConsole.Write(table);
                    
                    SystemController.CreateFileAndOpen(actionData.Value, actionData.Content ?? "", actionData.TargetApp);
                }
            }
        }
        catch (JsonException)
        {
            AnsiConsole.MarkupLine("[bold red]⚠️ Unexpected response:[/]");
            Console.WriteLine(message.Content);
        }
    }
    MemoryService.SaveHistory(conversationHistory);
    Console.WriteLine();
}