namespace LinkTracker.Scrapper.Domain.ClientsModels.StackOverflow;

public class StackOverflowResponse
{
    public List<QuestionItem> QuestionItems { get; set; } = new List<QuestionItem>();
}
