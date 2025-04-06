# AI Private Service
### Working with the Ollama Library in .NET

1. Install Ollama

Visit https://ollama.com and download the appropriate version for your operating system.

For Linux/macOS, you can install it via the terminal:
```bash
  curl -fsSL https://ollama.com/install.sh | sh
```

2. Start Ollama server:
```bash
    ollama serve
```

3. Download the Required Model (for example llama3)
```bash
    ollama pull llama3
```

4. Configure service via appsettings.json
Edit this or add to root of json config:
```json
  "AI": {
    "Endpoint": "http://localhost:12345/",
    "Model": "llama3"
  }
```