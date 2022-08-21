using CrisesControl.Api.Application.Commands.CCWebSocket.Get;
using CrisesControl.Api.Application.Commands.CCWebSocket.ProcessWebsocketSession;
using CrisesControl.Api.Application.Query;
using MediatR;
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
        private readonly IMediator _mediator;

        public CCWebSocketController(ILogger<CCWebSocketController> logger, ICCWebSocketQuery webSocketQuery, IMediator mediator)
        {
            _logger = logger;
            _webSocketQuery = webSocketQuery;
            _mediator = mediator;
        }
        /// <summary>
        /// Get ws
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("ws")]
        public async Task Get([FromRoute] GetRequest request)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                request.WebSocket = webSocket;
                _logger.Log(LogLevel.Information, "WebSocket connection established");
                await _webSocketQuery.Get(request);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
        /// <summary>
        /// Process Web Socket Session
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ProcessWebsocketSession")]
        public async Task<IActionResult> ProcessWebsocketSession([FromRoute] ProcessWebsocketSessionRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
