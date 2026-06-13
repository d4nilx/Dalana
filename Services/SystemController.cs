using System.Diagnostics;
using Spectre.Console;
using System.IO;
using System.Web;

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

    private static string GetTargetApp(string fileName, string? requestedApp)
    {
        string targetApp = requestedApp ?? "";
        string appLower = targetApp.ToLower();

        if (appLower.Contains("vs code") || appLower == "vscode") return "Visual Studio Code";
        if (appLower.Contains("rider")) return "Rider";

        if (string.IsNullOrEmpty(targetApp))
        {
            string extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".cs" => "Rider",
                ".py" => "Visual Studio Code", 
                ".txt" or ".md" or ".json" => "TextEdit",
                ".html" or ".css" => "Google Chrome",
                _ => "" 
            };
        }
        return targetApp;
    }
    
    public static void CreateFileAndOpen(string fileName, string fileContent, string? requestedApp = null)
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktop, fileName);
        
        string? directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string cleanContent = fileContent
            .Replace("\\n", "\n")
            .Replace("\\t", "\t");
        
        File.WriteAllText(filePath, cleanContent);

        string targetApp = GetTargetApp(fileName, requestedApp);

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
    
    public static void OpenFile(string fileName, string? requestedApp = null)
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktop, fileName);

        if (File.Exists(filePath))
        {
            string targetApp = GetTargetApp(fileName, requestedApp);

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
        else
        {
            AnsiConsole.MarkupLine($"[bold red]File not found:[/] {fileName}");
        }
    }
    
    public static void DraftEmail(string to, string subject, string body)
    {
        string encodedSubject = Uri.EscapeDataString(subject);
        string encodedBody = Uri.EscapeDataString(body);
        
        string mailtoUrl = $"mailto:{to}?subject={encodedSubject}&body={encodedBody}";

        Process.Start(new ProcessStartInfo
        {
            FileName = "open",
            Arguments = $"\"{mailtoUrl}\"",
            UseShellExecute = true
        });
    }
    
    private static string GetWorkspacePath()
    {
        string? envPath = Environment.GetEnvironmentVariable("DALANA_WORKSPACE");

        if (!string.IsNullOrEmpty(envPath) && Directory.Exists(envPath))
        {
            return envPath;
        }

        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string defaultWorkspace = Path.Combine(desktop, "DalanaWorkspace");

        if (!Directory.Exists(defaultWorkspace))
        {
            Directory.CreateDirectory(defaultWorkspace);
        }

        return defaultWorkspace;
    }

    public static string ReadSafeFile(string fileName)
    {
        string sandboxPath = GetWorkspacePath();

        string requestedPath = Path.GetFullPath(Path.Combine(sandboxPath, fileName));

        if (!requestedPath.StartsWith(sandboxPath))
        {
            AnsiConsole.MarkupLine("[bold red]🚨 Not happening!! AI does not have path here!![/]");
            return "[SECURITY ERROR] Access Denied. You are strictly confined to the workspace directory!!!!";
        }

        if (!File.Exists(requestedPath))
        {
            return $"[ERROR] File not found in workspace: {fileName}";
        }

        return File.ReadAllText(requestedPath);
    }
    
    public static void SendEmailReal(string to, string subject, string body)
    {
        try
        {
            string encodedSubject = Uri.EscapeDataString(subject);
            string encodedBody = Uri.EscapeDataString(body);

            string mailtoUrl = $"mailto:{to}?subject={encodedSubject}&body={encodedBody}";

            Process.Start(new ProcessStartInfo
            {
                FileName = "open",
                Arguments = $"\"{mailtoUrl}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SECURITY ERROR] Failed to open Mail app: {ex.Message}");
        }
    }
}