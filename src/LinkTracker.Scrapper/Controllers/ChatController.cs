using LinkTracker.Scrapper.Application.InterfacesServices;
using Microsoft.AspNetCore.Mvc;

namespace LinkTracker.Scrapper.Controllers;

[ApiController]
[Route("tg-chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("{id:long}")]
    public async Task<IActionResult> RegisterChat([FromRoute] long id, CancellationToken cancellationToken = default)
    {
        await _chatService.RegisterChatAsync(id, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteChat([FromRoute] long id, CancellationToken cancellationToken = default)
    {
        await _chatService.DeleteChatAsync(id, cancellationToken);
        return Ok();
    }
}
