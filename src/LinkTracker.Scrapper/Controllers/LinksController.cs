using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Contracts.Dto;
using Microsoft.AspNetCore.Mvc;

namespace LinkTracker.Scrapper.Controllers;

public class LinksController : ControllerBase
{
    private readonly ILinkService _linkService;

    public LinksController(ILinkService linkService)
    {
        _linkService = linkService; 
    }

    [HttpGet]
    public async Task<IActionResult> GetLinks([FromHeader(Name = "Tg-Chat-Id")] long chatId, [FromQuery] string? tag = null, CancellationToken cancellationToken = default)
    {
        var response = await _linkService.GetLinksAsync(chatId, tag, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> AddLink([FromHeader(Name = "Tg-Chat-Id")] long chatId, [FromBody] AddLinkRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _linkService.AddLinkAsync(chatId, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveLink([FromHeader(Name = "Tg-Chat-Id")] long chatId, [FromBody] RemoveLinkRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _linkService.RemoveLinkAsync(chatId, request, cancellationToken);
        return Ok(response);
    }
}
