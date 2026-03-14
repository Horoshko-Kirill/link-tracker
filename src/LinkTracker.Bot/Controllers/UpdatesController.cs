using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Telegram;
using Microsoft.AspNetCore.Mvc;

namespace LinkTracker.Bot.Controllers;

[ApiController]
[Route("updates")]
public class UpdatesController : ControllerBase
{
    private readonly ITelegramClient _telegramClient;

    public UpdatesController(ITelegramClient telegramClient)
    {
        _telegramClient = telegramClient; 
    }

    [HttpPost]
    public async Task<IActionResult> PostUpdate([FromBody] LinkUpdate linkUpdate, CancellationToken cancellationToken = default)
    {
        foreach(var chatId in linkUpdate.ChatIds)
        {
            var message = $"{linkUpdate.Url} : \n {linkUpdate.Description}";

            await _telegramClient.SendMessageAsync(chatId, message, cancellationToken);
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetUpdate(CancellationToken cancellationToken = default)
    {
        return Ok();
    }
}
