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

// if (conversationHistory.Count == 0)
// {
    conversationHistory.Clear();
    conversationHistory.Add(new ChatMessage
    {
        Role = "system",
        Content = @"You are Dalana, a strict macOS CLI system controller and intelligent assistant.
You MUST respond ONLY with a valid JSON object. NEVER explain anything outside JSON.

Your JSON must strictly match this format:
{
  ""thought"": ""<briefly explain your logic>"",
  ""actions"": [
    { ""action"": ""<action_type>"", ""value"": ""<target>"" }
  ]
}

Rules for actions:
1. 'open': ONLY for websites/URLs. Format: { ""action"": ""open"", ""value"": ""<target>"" }
2. 'open_app': ONLY for launching macOS apps. Format: { ""action"": ""open_app"", ""value"": ""<app_name>"" }
3. 'chat': for general answers/advice. Format: { ""action"": ""chat"", ""value"": ""<your_answer>"" }
4. 'create_file': include 'target_app' IF asked to open. Format: { ""action"": ""create_file"", ""value"": ""<file_name>"", ""content"": ""<text>"", ""target_app"": ""<app_name>"" }
5. 'open_file': ONLY to open existing files. Format: { ""action"": ""open_file"", ""value"": ""<file_name>"" }
6. 'draft_email': Format: { ""action"": ""draft_email"", ""to"": ""<email>"", ""subject"": ""<subject>"", ""body"": ""<body>"" }
7. 'read_file': Format: { ""action"": ""read_file"", ""value"": ""<file_name>"" }
8. Action 'preview_email': Use this only when you user want to send email AND you have all the necessary information (recipient email, name, context). Format: { ""action"": ""preview_email"", ""to"": ""<email>"", ""subject"": ""<subject>"", ""body"": ""<body>"" }
9. Action 'save_memory': Use this to save important user data for the future (like a professor's email or a friend's contact). Format: { ""action"": ""save_memory"", ""value"": ""Prof. Kowalski's email is kowalski@wsb.pl"" }

CRITICAL EXAMPLES:
User: 'open youtube and Calculator' -> 
{
  ""thought"": ""User wants to open a website and a local app."",
  ""actions"": [
    { ""action"": ""open"", ""value"": ""youtube.com"" },
    { ""action"": ""open_app"", ""value"": ""Calculator"" }
  ]
}
CRITICAL EMAIL RULE: If the user asks to send an email but you DO NOT know the recipient's email address or name, YOU MUST NOT use 'preview_email'. Instead, use the 'chat' action to ask the user for the missing details. Once the user provides them, first use 'save_memory' to remember the contact, and then use 'preview_email'."
    });

    MemoryService.SaveHistory(conversationHistory);
// }

Console.Clear();

string startupWord = "DALANA";
string currentStartupText = "";

AnsiConsole.Live(new FigletText("").LeftJustified().Color(Color.SpringGreen3))
    .Start(ctx => 
    {
        foreach (char letter in startupWord)
        {
            currentStartupText += letter;
            
            ctx.UpdateTarget(
                new FigletText(currentStartupText)
                    .LeftJustified()
                    .Color(Color.SpringGreen3)
            );
            
            Thread.Sleep(50); 
        }
    });
Thread.Sleep(300); 

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
    
        string targetWord = "OFFLINE";
        string currentText = "";

        AnsiConsole.Live(new FigletText("").Centered().Color(Color.Red))
            .Start(ctx => 
            {
                foreach (char letter in targetWord)
                {
                    currentText += letter; 
                
                    ctx.UpdateTarget(new FigletText(currentText).Centered().Color(Color.Red));
                
                    Thread.Sleep(50); 
                }
            });

        Thread.Sleep(300); 
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
        Model = "openai/gpt-oss-120b",
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

            DalanaResponse? responseData = JsonSerializer.Deserialize<DalanaResponse>(cleanJson);

            if (responseData != null)
            {
                if (responseData.Actions != null)
                {
                    foreach (var actionData in responseData.Actions)
                    {
                        if (actionData.Action == "open")
                        {
                            SystemController.OpenSmart(actionData.Value);
                        }
                        else if (actionData.Action == "open_app")
                        {
                            SystemController.OpenApp(actionData.Value);
                        }
                        else if (actionData.Action == "open_file")
                        {
                            SystemController.OpenFile(actionData.Value);
                        }
                        else if (actionData.Action == "chat")
                        {
                            AnsiConsole.MarkupLine($"[bold blue]Dalana:[/] {Markup.Escape(actionData.Value)}");
                        }
                        else if (actionData.Action == "create_file")
                        {
                            SystemController.CreateFileAndOpen(actionData.Value, actionData.Content ?? "",
                                actionData.TargetApp);
                            AnsiConsole.MarkupLine($"[green]✅ File created:[/] {Markup.Escape(actionData.Value)}");
                        }
                        else if (actionData.Action == "draft_email")
                        {
                            SystemController.DraftEmail(actionData.To ?? "", actionData.Subject ?? "",
                                actionData.Body ?? "");
                            AnsiConsole.MarkupLine(
                                $"[green]✅ Email drafted to:[/] {Markup.Escape(actionData.To ?? "")}");
                        }
                        else if (actionData.Action == "save_memory")
                        {
                            File.AppendAllText("memory.txt", actionData.Value + Environment.NewLine);
                            AnsiConsole.MarkupLine($"[dim green] Memory updated:[/] {Markup.Escape(actionData.Value)}");
                        }
                        else if (actionData.Action == "preview_email")
                        {
                            var emailPanel =
                                new Panel(
                                    $"[bold]To:[/] {Markup.Escape(actionData.To ?? "")}\n[bold]Subject:[/] {Markup.Escape(actionData.Subject ?? "")}\n\n{Markup.Escape(actionData.Body ?? "")}")
                                {
                                    Header = new PanelHeader("[blue]📧 Email Draft Preview[/]"),
                                    Padding = new Padding(1, 1, 1, 1),
                                    Border = BoxBorder.Rounded
                                };
                            AnsiConsole.Write(emailPanel);

                            if (AnsiConsole.Confirm($"[bold red] You sure you want to send this email?[/]"))
                            {
                                SystemController.SendEmailReal(actionData.To ?? "", actionData.Subject ?? "", actionData.Body ?? "");
                                AnsiConsole.MarkupLine("[bold green]✅ Mail sent![/]");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"[yellow] Sending canceled. [/]");
                            }
                        }
                        else if (actionData.Action == "read_file")
                        {
                            string fileContent = SystemController.ReadSafeFile(actionData.Value);

                            if (fileContent.StartsWith("[SECURITY ERROR]") || fileContent.StartsWith("[ERROR]"))
                            {
                                AnsiConsole.MarkupLine($"[bold red]{Markup.Escape(fileContent)}[/]");
                            }
                            else
                            {
                                var fileContextMessage = new ChatMessage
                                {
                                    Role = "user",
                                    Content =
                                        $"[SYSTEM INJECTION] Here is the content of the file '{actionData.Value}':\n\n```\n{fileContent}\n```\n\nPlease acknowledge you read it and answer my previous request about this file."
                                };
                                conversationHistory.Add(fileContextMessage);

                                AnsiConsole.MarkupLine(
                                    $"[dim green]File {Markup.Escape(actionData.Value)} loaded into memory.[/]");
                            }
                        }
                    }
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