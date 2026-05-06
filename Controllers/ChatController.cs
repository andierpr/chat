using AIChatRailway.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIChatRailway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly OpenAIService _ai;

        public ChatController(OpenAIService ai)
        {
            _ai = ai;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Mensagem é obrigatória.");

            var response = await _ai.AskAsync(request.Message);

            return Ok(new { response });
        }
    }

    public class ChatRequest
    {
        public required string Message { get; set; }
    }
}