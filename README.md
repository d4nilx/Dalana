# 🤖 Dalana

> A local AI assistant CLI that runs entirely on your Mac — no cloud, no API keys, no internet required.

Dalana talks to a locally running [Ollama](https://ollama.com) instance and gives you AI responses directly in your terminal. Built for developers who want a fast, private, and extensible AI tool they actually own.

---

## Why Dalana?

- **100% local** — powered by Llama 3 via Ollama, everything stays on your machine
- **No subscriptions** — free forever, no rate limits
- **Extensible** — built to grow into a full developer agent (file editing, app launching, browser automation)

---

## Requirements

- macOS (Apple Silicon recommended)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Ollama](https://ollama.com) installed and running
- Llama 3 model pulled: `ollama pull llama3`

---

## Installation

```bash
git clone https://github.com/d4nilx/Dalana.git
cd Dalana
dotnet run
```

Make sure Ollama is running before you start:
```bash
ollama serve
```

---

## Usage

```bash
dotnet run
```

```
What's your request today?: explain what async/await does in C#
Sending your request...

Answer:
async/await is a pattern that allows you to write asynchronous code
that looks and behaves like synchronous code...
```

---

## Project Structure

```
Dalana/
├── Program.cs
├── Models/
│   ├── OllamaRequest.cs    ← request payload sent to Ollama
│   └── OllamaResponse.cs   ← parsed response from Ollama
└── Services/
    └── OllamaService.cs    ← (coming soon) HTTP client logic
```

---

## Roadmap

- [x] Basic Ollama API communication
- [x] JSON serialization / deserialization
- [ ] Spectre.Console UI with spinner and chat loop
- [ ] System controller — open apps and URLs via `Process.Start`
- [ ] Function calling — AI returns structured JSON actions
- [ ] File manipulation — read and rewrite code files via AI
- [ ] AppleScript automation — simulate keyboard input

---

## Tech Stack

- **C# / .NET 10** — console app with `HttpClient` and `System.Text.Json`
- **Ollama** — local LLM runtime
- **Llama 3** — default model

---

## Status

🚧 Early development — Phase 1 complete (AI core), Phase 2 in progress.

---

*Built by [Daniil Zhdanov / @d4nilx](https://github.com/d4nilx)*
