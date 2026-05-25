using LinkTracker.AiAgent.Domain.Models;
using LinkTracker.Bot.Contracts.Avro;

namespace LinkTracker.AiAgent.Application.InterfacesServices;

public interface IMessageSender
{
    Task SendAsync(LinkUpdateEvent linkUpdateEvent, CancellationToken cancellationToken = default);
}