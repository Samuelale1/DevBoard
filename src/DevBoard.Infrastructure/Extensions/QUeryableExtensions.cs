using DevBoard.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
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