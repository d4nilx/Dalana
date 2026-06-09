using System.Diagnostics;
using Spectre.Console;

namespace Dalana.Services;

public class SystemController
{
    private static readonly Dictionary<string, string> AppAliases = new()
    {
        { "word", "Microsoft Word" },
        { "excel", "Microsoft Excel" },
        { "powerpoint", "Microsoft PowerPoint" },
        { "spark", "Spark – Email App by Readdle" },
        { "rider", "Rider" },
        { "chrome", "Google Chrome" },
        { "safari", "Safari" },
        { "terminal", "Terminal" },
        { "calculator", "Calculator" },
        { "notes", "Notes" },
        { "music", "Music" },
    };
    
    public static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "open",
            Arguments = url,
            UseShellExecute = true
        });
    }
    
    public static void OpenSmart(string query)
    {
        string cleanQuery = query.ToLower().Trim();

        if (cleanQuery.StartsWith("http://") || cleanQuery.StartsWith("https://"))
        {
            OpenUrl(cleanQuery);
        }
        
        else if (cleanQuery.Contains("."))
        {
            OpenUrl($"https://{cleanQuery}");
        }
        else
        {
            string encoded = Uri.EscapeDataString(query);
            OpenUrl($"https://www.google.com/search?q={encoded}");
        }
    }

    public static void OpenApp(string appName)
    {
        string resolvedName = AppAliases.TryGetValue(
            appName.ToLower(), out string? alias) ? alias : appName;
    
        Process.Start(new ProcessStartInfo
        {
            FileName = "open",
            Arguments = $"-a \"{resolvedName}\"",
            UseShellExecute = true,
        });
    }
    
    public static string FormatUrl(string query)
    {
        string cleanQuery = query.ToLower().Trim();

        if (cleanQuery.StartsWith("http://") || cleanQuery.StartsWith("https://")) return cleanQuery;
        if (cleanQuery.Contains(".")) return $"https://{cleanQuery}";
        
        string domain = cleanQuery.Replace(" ", ""); 
        return $"https://{domain}.com";
    }
    
    public static void CreateFileAndOpen(string fileName, string fileContent, string? requestedApp = null)
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktop, fileName);

        string cleanContent = fileContent
            .Replace("\\n", "\n")
            .Replace("\\t", "\t");
        
        File.WriteAllText(filePath, cleanContent);

        string targetApp = requestedApp ?? "";

        if (string.IsNullOrEmpty(targetApp))
        {
            string extension = Path.GetExtension(fileName).ToLower();
            targetApp = extension switch
            {
                ".cs" => "Rider",
                ".py" => "Visual Studio Code", 
                ".txt" or ".md" or ".json" => "TextEdit",
                ".html" or ".css" => "Google Chrome",
                _ => "" 
            };
        }

        string arguments = string.IsNullOrEmpty(targetApp) 
            ? $"\"{filePath}\"" 
            : $"-a \"{targetApp}\" \"{filePath}\"";

        Process.Start(new ProcessStartInfo
        {
            FileName = "open",
            Arguments = arguments,
            UseShellExecute = true
        });
    }
    
    public static void OpenFile(string fileName)
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktop, fileName);

        if (File.Exists(filePath))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "open",
                Arguments = $"\"{filePath}\"",
                UseShellExecute = true
            });
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold red]File not found:[/] {fileName}");
        }
    }
}