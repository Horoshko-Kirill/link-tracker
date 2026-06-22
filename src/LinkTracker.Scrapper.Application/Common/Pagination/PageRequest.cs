namespace LinkTracker.Scrapper.Application.Common.Pagination;

public class PageRequest
{
    public long LastId { get; }
    public int Size { get; }
    public PageRequest(long lastId, int size)
    {
        if (lastId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lastId));
        }

        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        LastId = lastId;
        Size = size;
    }
}
