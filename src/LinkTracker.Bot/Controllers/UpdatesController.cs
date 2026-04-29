using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Contracts.Dto;
using Microsoft.AspNetCore.Mvc;

namespace LinkTracker.Bot.Controllers;

[ApiController]
[Route("updates")]
public class UpdatesController : ControllerBase
{
    private readonly ILinkUpdateHandler _handler;

    public UpdatesController(ILinkUpdateHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> PostUpdate([FromBody] LinkUpdate linkUpdate, CancellationToken cancellationToken = default)
    {
        await _handler.HandleAsync(linkUpdate, cancellationToken);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetUpdate(CancellationToken cancellationToken = default)
    {
        return Ok();
    }
}
