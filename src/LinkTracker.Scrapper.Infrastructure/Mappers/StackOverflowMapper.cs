using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Contracts.Dto.StackOverflow;
using LinkTracker.Scrapper.Domain.Enum;
using LinkTracker.Scrapper.Infrastructure.Helpers;

namespace LinkTracker.Scrapper.Infrastructure.Mappers;

public static class StackOverflowMapper
{
    public static UpdateEventDto MapAnswer(StackOverflowAnswerResponse answer, string questionTitle)
    {
        return new UpdateEventDto
        {
            EventType = StackOverflowType.Answer.ToString(),
            Source = UpdateSource.StackOverflow.ToString(),
            Title = questionTitle,
            Author = answer.Owner?.DisplayName ?? string.Empty,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(answer.CreationDate),
            Preview = PreviewHelper.Build(answer.Body)
        };
    }

    public static UpdateEventDto MapComment(StackOverflowCommentResponse comment, string questionTitle)
    {
        return new UpdateEventDto
        {
            EventType = StackOverflowType.Comment.ToString(),
            Source = UpdateSource.StackOverflow.ToString(),
            Title = questionTitle,
            Author = comment.Owner?.DisplayName ?? string.Empty,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(comment.CreationDate),
            Preview = PreviewHelper.Build(comment.Body)
        };
    }
}