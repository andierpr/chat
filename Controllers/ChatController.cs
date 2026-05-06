using AIChatRailway.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIChatRailway.Controllers
{
    [ApiController]
    [Route("chat")]
    public class ChatController : ControllerBase
    {
        private readonly OpenAIService _ai;

        public ChatController(OpenAIService ai)
        {
            _ai = ai;
        }

        [HttpGet("debug")]
        public IActionResult Debug()
        {
            var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            return Ok(new
            {
                exists = !string.IsNullOrEmpty(key),
                length = key?.Length ?? 0,
                preview = key is not null ? key[..Math.Min(8, key.Length)] + "..." : null
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { error = "Mensagem é obrigatória." });

            try
            {
                var response = await _ai.AskAsync(request.Message);
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Erro ao processar IA",
                    detail = ex.Message
                });
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}