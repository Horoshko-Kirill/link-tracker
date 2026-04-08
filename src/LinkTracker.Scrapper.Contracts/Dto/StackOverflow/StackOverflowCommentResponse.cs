using System.Text.Json.Serialization;

namespace LinkTracker.Scrapper.Contracts.Dto.StackOverflow;

public class StackOverflowCommentResponse
{
    [JsonPropertyName("comment_id")]
    public long CommentId { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("creation_date")]
    public long CreationDate { get; set; }

    [JsonPropertyName("owner")]
    public StackOverflowOwnerResponse? Owner { get; set; }
}