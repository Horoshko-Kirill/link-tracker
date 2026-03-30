namespace LinkTracker.Scrapper.Contracts.Dto
{
    public class LinkResponse
    {
        public long ChatId { get; set; }
        public string Url { get; set; } = null!;
        public List<string> Tags { get; set; } = new List<string>();
    }
}
