# 🤖 Dalana

> A blazing fast macOS CLI AI assistant and system agent powered by the Groq API.

Dalana is an intelligent, context-aware assistant that lives in your terminal. It doesn't just chat — it acts as a system agent capable of opening URLs, launching macOS applications, writing code, creating files, and smartly routing them to the right IDEs (like Rider or VS Code). Built for developers who want maximum terminal productivity.

---

## Why Dalana?

- **Blazing Fast** — powered by the `llama-3.3-70b-versatile` model via Groq API for near-instant responses.
- **System-Aware** — native macOS integration to control files and apps.
- **Smart Routing** — knows to open `.cs` files in Rider, `.py` in VS Code, and standard text in TextEdit.
- **Persistent Memory** — remembers your conversation context across different terminal sessions.

---

## Requirements

- macOS (Apple Silicon recommended)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A free API key from [Groq](https://console.groq.com/keys)

---

## Installation

```bash
# Clone the repository
git clone [https://github.com/d4nilx/Dalana.git](https://github.com/d4nilx/Dalana.git)
cd Dalana

# Set up your environment variables
echo "GROQ_API_KEY=your_api_key_here" > .env

# Run the application
dotnet run
```

---

## Usage

Just launch the app and start typing. Dalana understands natural language intents and translates them into system actions.

```bash
dotnet run
```

**Examples of what you can ask:**
- *"youtube"* ➔ Opens youtube.com in your default browser.
- *"launch rider"* ➔ Opens JetBrains Rider.
- *"write a C# script to sum two numbers"* ➔ Generates `Program.cs` and opens it in Rider.
- *"open script.py in VS Code"* ➔ Finds the file and opens it in your specified editor.
- *"explain async/await"* ➔ Explains it directly in the CLI chat.

---

## Project Structure

```text
Dalana/
├── Program.cs              ← Main loop, Groq API integration & Spectre.Console UI
├── .env                    ← Your local API keys (gitignored)
├── Models/
│   ├── GroqRequest.cs      ← Request payload for the LLM
│   ├── GroqResponse.cs     ← Parsed API response
│   ├── GroqActions.cs      ← Structured JSON schema for function calling
│   └── ChatMessage.cs      ← Message history model
└── Services/
    ├── SystemController.cs ← macOS system execution, smart file routing
    └── MemoryService.cs    ← Handles local chat history storage
```

---

## Roadmap

- [x] Basic API communication (migrated to Groq)
- [x] JSON serialization / deserialization
- [x] Spectre.Console UI with spinner and chat loop
- [x] Persistent local memory/history
- [x] System controller — open apps and URLs via `Process.Start`
- [x] Function calling — AI returns structured JSON actions
- [x] File manipulation — create, edit, and open files with smart IDE routing
- [ ] Automated email drafting via macOS protocols
- [ ] AppleScript automation — simulate deeper system controls

---

## Tech Stack

- **C# / .NET 10** — core application
- **Spectre.Console** — beautiful CLI rendering
- **DotNetEnv** — environment variable management
- **Groq API (`gpt-oss-120b`)** — LLM backend for instant structured JSON output

---

## Status

🚧 Active development — Phase 3 (System Agent & File Management) complete. Moving to Phase 4 (Deep macOS Integrations).

---

*Built by [Daniil Zhdanov / @d4nilx](https://github.com/d4nilx)*