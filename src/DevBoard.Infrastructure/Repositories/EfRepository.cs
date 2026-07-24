using DevBoard.Domain.Entities;
using DevBoard.Domain.Interfaces;
using DevBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Repositories;

public sealed class EfRepository<T> : IRepository<T>
    where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public EfRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _dbSet.FindAsync([id], ct);
    }

    public IQueryable<T> Query()
    {
        return _dbSet.AsNoTracking();
    }

    public async Task AddAsync(
        T entity,
        CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task SaveChangesAsync(
        CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}