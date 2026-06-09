using System.Text.Json;
using Xunit;
using Dalana.Models;
using Dalana.Services;

namespace Dalana.Tests;

public class AgentLogicTests
{
    [Fact]
    public void LlamaAction_ShouldParse_AppLaunchRequest()
    {
        // 1. Arrange 
        string aiResponse = @"{ ""action"": ""open_app"", ""value"": ""Calculator"" }";

        // 2. Act 
        LlamaAction? result = JsonSerializer.Deserialize<LlamaAction>(aiResponse);

        // 3. Assert 
        Assert.NotNull(result);
        Assert.Equal("open_app", result.Action);
        Assert.Equal("Calculator", result.Value);
    }

    [Fact]
    public void LlamaAction_ShouldParse_ChatConversation()
    {
        // 1. Arrange 
        string aiResponse = @"{ ""action"": ""chat"", ""value"": ""async/await is used for asynchronous programming."" }";

        // 2. Act 
        LlamaAction? result = JsonSerializer.Deserialize<LlamaAction>(aiResponse);

        // 3. Assert 
        Assert.NotNull(result);
        Assert.Equal("chat", result.Action);
        Assert.Contains("async/await", result.Value);
    }

    [Theory] 
    [InlineData("youtube.com", "https://youtube.com")]
    [InlineData("https://github.com", "https://github.com")]
    [InlineData("expedia", "https://expedia.com")]
    [InlineData("booking site", "https://bookingsite.com")]
    public void SystemController_ShouldFormatUrlCorrectly(string input, string expectedOutput)
    {
        string result = SystemController.FormatUrl(input);
        
        Assert.Equal(expectedOutput, result);
    }
}