using System.Diagnostics;

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
        
        else if (cleanQuery.Contains("."))
        {
            string domain = cleanQuery.Replace(" ", "");
            OpenUrl($"https://{domain}.com");
            Console.WriteLine($"[System] Opening: https://{domain}.com");
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
}