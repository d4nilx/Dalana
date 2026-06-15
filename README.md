# 🤖 Dalana

> A blazing fast, hybrid macOS CLI system agent powered by Groq API and local Ollama.

Dalana is an intelligent, context-aware assistant that lives in your terminal. It doesn't just chat — it acts as a system agent capable of opening URLs, launching macOS applications, writing code, reading files, and drafting emails.

Built with a **Hybrid AI Routing** architecture, Dalana dynamically switches between a lightning-fast local LLM (for simple system tasks) and a powerful cloud model (for complex generation) to save resources and ensure maximum privacy.

---

## ✨ Key Features

- 🧠 **Hybrid AI Routing** — Automatically routes complex tasks (coding, email drafting) to **Groq API** and simple tasks (opening apps, creating files) to a local **Ollama** model.
- ⚡ **Blazing Fast** — Built on .NET 10 with asynchronous execution.
- 💻 **System-Aware** — Native macOS integration to control files, apps, and communication.
- 📂 **Smart File Handling** — Creates files, opens existing ones, and can read file contents securely into its context.
- 📧 **Human-in-the-Loop Emails** — Drafts professional emails, saves unknown contacts to persistent memory, and requests interactive `[y/n]` user confirmation before securely opening the macOS Mail app.
- 💾 **Persistent Memory** — Remembers conversation context, contacts, and user facts across different terminal sessions.

---

## 🛠️ Requirements

- macOS (Apple Silicon M-series highly recommended)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Ollama](https://ollama.com/) installed and running locally.
- A free API key from [Groq](https://console.groq.com/keys)

---

## 🚀 Installation

git clone https://github.com/d4nilx/Dalana.git
cd Dalana

echo "GROQ_API_KEY=your_api_key_here" > .env

ollama run llama3

dotnet run

---

## 💡 Usage

Launch the app and start typing naturally. Dalana parses your intent and executes structured JSON actions under the hood.

dotnet run

**Example Scenarios:**
- *"youtube"* ➔ Quickly opens youtube.com using the local model.
- *"launch rider"* ➔ Opens JetBrains Rider locally.
- *"write a C# script to sum two numbers"* ➔ Routes to Groq, generates code, saves it to a file, and opens it.
- *"read index.html and find bugs"* ➔ Reads local file contents directly into the AI's context.
- *"email professor Kowalski that I'll be late"* ➔ AI checks memory for the email, drafts a message, shows a preview, and asks for your permission before executing the `mailto:` protocol.

---

## 🏗️ Project Architecture

Dalana uses a modular structure with a focus on separation of concerns:

```text
Dalana/
├── Program.cs              ← Main loop, Hybrid AI Router & Spectre.Console UI
├── .env                    ← Your local API keys (gitignored)
├── memory.txt              ← Persistent contact and fact storage
├── Models/
│   ├── DalanaResponse.cs   ← Universal JSON schema handling Chain-of-Thought
│   ├── GroqAction.cs       ← Action definitions (open, create_file, preview_email, etc.)
│   ├── GroqRequest.cs      ← Request payload for the cloud LLM
│   ├── GroqResponse.cs     ← Parsed API response from Groq
│   └── ChatMessage.cs      ← Message history model
└── Services/
    ├── OllamaService.cs    ← Handles local LLM requests ensuring strict JSON format
    ├── SystemController.cs ← macOS system execution, file handling, and email protocols
    └── MemoryService.cs    ← Handles chat history serialization
```
---

## 🗺️ Roadmap

- [x] Basic API communication (migrated to Groq)
- [x] Spectre.Console UI with animated spinners
- [x] Function calling — AI returns structured JSON actions (`Chain-of-Thought` pattern)
- [x] **Phase 3:** System controller — app launching and smart file creation
- [x] **Phase 4:** Human-in-the-loop email drafting and persistent contact memory
- [x] **Phase 5:** Hybrid AI Routing (Groq Cloud + Ollama Local)
- [ ] **Phase 6:** Telegram Bot integration for remote macOS control
- [ ] Playwright integration for automated web scraping/booking

---

## 💻 Tech Stack

- **C# / .NET 10** — Core application
- **Spectre.Console** — Beautiful, interactive CLI rendering
- **DotNetEnv** — Environment variable management
- **Groq API (`openai/gpt-oss-120b`)** — Cloud backend for heavy reasoning
- **Ollama (`llama3`)** — Local backend for fast, offline system commands

---

*Built by [Daniil Zhdanov / @d4nilx](https://github.com/d4nilx) | DevOps & .NET Enthusiast*