using CrisesControl.Api.Application.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CCWebSocketController : ControllerBase
    {
        private readonly ILogger<CCWebSocketController> _logger;
        private readonly ICCWebSocketQuery _webSocketQuery;

        public CCWebSocketController(ILogger<CCWebSocketController> logger, ICCWebSocketQuery webSocketQuery)
        {
            _logger = logger;
            _webSocketQuery = webSocketQuery;
        }
        [HttpGet("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                _logger.Log(LogLevel.Information, "WebSocket connection established");
                await _webSocketQuery.Get(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
