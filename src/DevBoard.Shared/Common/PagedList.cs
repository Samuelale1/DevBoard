namespace DevBoard.Shared.Common;

public sealed class PagedList<T>
{
    public IReadOnlyList<T> Items { get; }

    public int TotalCount { get; }

    public int CurrentPage { get; }

    public int PageSize { get; }

    public int TotalPages { get; }

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;

    public PagedList(
        IReadOnlyList<T> items,
        int totalCount,
        int currentPage,
        int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;

        TotalPages = (int)Math.Ceiling(
            totalCount / (double)pageSize);
    }
}