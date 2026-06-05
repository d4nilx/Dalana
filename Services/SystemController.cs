using System.Diagnostics;

namespace Dalana.Services;

public class SystemController
{
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
            string domain = cleanQuery.Replace(" ", ""); 
            
            OpenUrl($"https://{domain}.com");
            Console.WriteLine($"[System] Opening: https://{domain}.com");
        }
    }

    public static void OpenApp(string appName)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "open",
            Arguments = $"-a \"{appName}\"",
            UseShellExecute = true,
        });
    }
}