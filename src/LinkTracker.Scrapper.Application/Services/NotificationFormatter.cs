using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.Services;

public class NotificationFormatter : INotificationFormatter
{
    public string Format(UpdateEvent updateEvent, string url)
    {
        return updateEvent.Source switch
        {
            "GitHub" => $"""
                         Обновление GitHub

                         Ссылка: {url}
                         Тип: {updateEvent.EventType}
                         Название: {updateEvent.Title}
                         Автор: {updateEvent.Author}
                         Время создания: {updateEvent.CreatedAt:O}

                         Превью:
                         {updateEvent.Preview}
                         """,

            "StackOverflow" => $"""
                                Обновление StackOverflow

                                Ссылка: {url}
                                Тема вопроса: {updateEvent.Title}
                                Автор: {updateEvent.Author}
                                Время создания: {updateEvent.CreatedAt:O}

                                Превью:
                                {updateEvent.Preview}
                                """,

            _ => $"""
                  Обновление

                  Ссылка: {url}
                  Тип: {updateEvent.EventType}
                  Заголовок: {updateEvent.Title}
                  Автор: {updateEvent.Author}
                  Время создания: {updateEvent.CreatedAt:O}

                  Превью:
                  {updateEvent.Preview}
                  """
        };
    }
}