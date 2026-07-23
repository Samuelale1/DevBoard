using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Common;

public sealed class PagedList<T>
{
    public IReadOnlyList<T> Items { get; }

    public int TotalCount { get; }

    public int CurrentPage { get; }

    public int PageSize { get; }

    public int TotalPages { get; }

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;

    private PagedList(
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

    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> query,
        int page,
        int size,
        CancellationToken ct = default)
    {
        var count = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        return new PagedList<T>(
            items,
            count,
            page,
            size);
    }
}