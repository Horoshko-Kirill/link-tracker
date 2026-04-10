using System.Text;
using LinkTracker.Scrapper.Application.Common.Results;
using LinkTracker.Scrapper.Application.InterfacesServices;

namespace LinkTracker.Scrapper.Application.Services;

public class ReportFormatter : IReportFormatter
{
    public string Format(IReadOnlyCollection<LinkProcessingResult> failedLinks, DateTimeOffset scanStartedAt, DateTimeOffset scanFinishedAt)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("Отчет о проверке ссылок");
        sb.AppendLine();
        sb.AppendLine($"Начало сканирования: {scanStartedAt:O}");
        sb.AppendLine($"Окончание сканирования: {scanFinishedAt:O}");
        sb.AppendLine();
        sb.AppendLine($"Не удалось обработать ссылок: {failedLinks.Count}");
        sb.AppendLine();

        var index = 1;
        foreach (var failedLink in failedLinks)
        {
            sb.AppendLine($"{index}. {failedLink.Url}");
            sb.AppendLine($"   Причина: {failedLink.ErrorMessage ?? "Unknown error"}");
            sb.AppendLine();
            index++;
        }
        
        return sb.ToString();
    }
}