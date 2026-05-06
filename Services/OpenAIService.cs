using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

public async Task<string> AskAsync(string message)
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

    if (string.IsNullOrWhiteSpace(apiKey))
        return "ERRO: OPENAI_API_KEY não configurada no Railway.";

    var body = new
    {
        model = "gpt-4o-mini",
        messages = new[]
        {
            new { role = "system", content = "Você é um assistente técnico objetivo." },
            new { role = "user", content = message }
        }
    };

    var request = new HttpRequestMessage(HttpMethod.Post,
        "https://api.openai.com/v1/chat/completions");

    request.Headers.Authorization =
        new AuthenticationHeaderValue("Bearer", apiKey);

    request.Content = new StringContent(
        JsonSerializer.Serialize(body),
        Encoding.UTF8,
        "application/json"
    );

    var response = await _http.SendAsync(request);
    var json = await response.Content.ReadAsStringAsync();

    // 🔥 TRATAMENTO DE ERRO (ESSENCIAL)
    if (!response.IsSuccessStatusCode)
    {
        return $"Erro OpenAI: {response.StatusCode} - {json}";
    }

    try
    {
        using var doc = JsonDocument.Parse(json);

        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return content ?? "Sem resposta";
    }
    catch
    {
        return "Erro ao processar resposta da IA.";
    }
}