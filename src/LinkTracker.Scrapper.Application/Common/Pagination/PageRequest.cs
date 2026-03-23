namespace LinkTracker.Scrapper.Application.Common.Pagination;

public class PageRequest
{
    public int Offset { get; }
    public int Size { get; }
    public PageRequest(int offset, int size)
    {
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        Offset = offset;
        Size = size;
    }
}
