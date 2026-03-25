using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.Mappers;

public static class TagMappers
{
    public static List<Tag> ToDomain(List<string> tags)
    {
        return tags
            .Where(t => !string.IsNullOrEmpty(t))
            .Select(t => t.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(t => new Tag
            {
                Name = t,
            })
            .ToList();
    }
}

