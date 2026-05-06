using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AIChatRailway.Services;

public class OpenAIService
{
    private readonly HttpClient _http;

    public OpenAIService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> AskAsync(string message)
    {
        var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("GROQ_API_KEY não configurada no Railway.");

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.groq.com/openai/v1/chat/completions"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var body = new
        {
            model = "llama3-8b-8192",
            messages = new[]
            {
                new { role = "system", content = "Você é um assistente técnico objetivo." },
                new { role = "user", content = message }
            }
        };

        request.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        // 🔥 1. TRATA ERRO DA API
        if (doc.RootElement.TryGetProperty("error", out var error))
        {
            var msg = error.GetProperty("message").GetString();
            throw new Exception($"Groq error: {msg}");
        }

        // 🔥 2. VALIDA choices
        if (!doc.RootElement.TryGetProperty("choices", out var choices) ||
            choices.GetArrayLength() == 0)
        {
            throw new Exception("Resposta inválida da IA (sem choices).");
        }

        // 🔥 3. EXTRAI RESPOSTA COM SEGURANÇA
        var content =
            choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

        return content ?? "Sem resposta da IA";
    }
}