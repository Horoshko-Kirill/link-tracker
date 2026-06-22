using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface INotificationFormatter
{
    string Format(UpdateEvent updateEvent, string url);
}