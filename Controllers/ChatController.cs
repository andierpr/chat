using AIChatRailway.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIChatRailway.Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    private readonly OpenAIService _service;

    public ChatController(OpenAIService service)
    {
        _service = service;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("API funcionando");
    }
    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        var result = await _service.AskAsync(request.Message);
        return Ok(new { response = result });
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
}